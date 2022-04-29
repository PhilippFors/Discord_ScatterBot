using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScatterBot_v2.core;
using ScatterBot_v2.core.ErrorHandling;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Helpers;
using ScatterBot_v2.Data;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2
{
    public class Program
    {
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private Services services;
        private ErrorHandler errorHandler;

        // Entry point
        public static void Main() =>
            new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {   // get token from text file
            var stream = File.OpenRead("token.txt");
            var token = await new StreamReader(stream).ReadToEndAsync();
            await stream.DisposeAsync();

            _client = new DiscordClient(
                new () {
                    Token = token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    GatewayCompressionLevel = GatewayCompressionLevel.Payload,
                    Intents = DiscordIntents.Guilds | DiscordIntents.GuildBans | DiscordIntents.GuildEmojis |
                              DiscordIntents.GuildMessages | DiscordIntents.GuildPresences | DiscordIntents.Guilds |
                              DiscordIntents.GuildMembers,
                    MessageCacheSize = 300,
                    MinimumLogLevel = LogLevel.Debug
                }
            );

            var applicationHandler = new ApplicationHandler();
            
            var saveSystem = new SaveSystem();
            saveSystem.LoadData();
            
            await InitializeHandlers(saveSystem, applicationHandler);
            await _client.ConnectAsync();
            
            Guild.guild = await _client.GetGuildAsync(Guild.guildId);
            await services.saveSystem.InitializeRuntimeData(Guild.guild);

            _client.ChannelPinsUpdated += HandleChannelPinsUpdated;
            _client.MessageCreated += HandleMessagesCreated;
            _client.GuildMemberAdded += (ctx, args) => services.newUserHelper.AddUser(args.Member.Id);
            
            // exit when program close
            await Task.Delay(-1, applicationHandler.ctx.Token);
        }

        private Task InitializeHandlers(SaveSystem saveSystem, ApplicationHandler handler)
        {
            services = new Services() {
                saveSystem = saveSystem,
                bonkedHelper = new BonkedHelper(saveSystem),
                newUserHelper = new NewUserHelper(saveSystem),
                pinHelper = new PinHelper(saveSystem),
                applicationHandler = handler
            };

            var di = new ServiceCollection()
                .AddSingleton(services.bonkedHelper)
                .AddSingleton(services.newUserHelper)
                .AddSingleton(services.pinHelper)
                .AddSingleton(services.saveSystem)
                .AddSingleton(services.applicationHandler)
                .BuildServiceProvider();

            var config = new CommandsNextConfiguration() {
                StringPrefixes = new[] {"!"},
                CaseSensitive = true,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = false,
                Services = di
            };
            
            errorHandler = new ErrorHandler(services, _client);
            
            _commands = _client.UseCommandsNext(config);
            _commands.RegisterCommands(Assembly.GetEntryAssembly());
            _commands.CommandErrored += errorHandler.HandleCommandErrored;

            return Task.CompletedTask;
        }

        private async Task HandleMessagesCreated(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message.Channel.Id == services.saveSystem.ServerData.welcomeChannel) {
                // Bonked users can still type in welcome so this bonks 'em again
                var author = message.Author;

                if (services.bonkedHelper.IsBonked(author.Id)) {
                    var response = await message.Channel.SendMessageAsync($"No talking for you {author.Mention}.");
                    await message.DeleteAsync();
                    await response.WaitDeleteMessage(15);
                }

                // new user messages will be logged and can be used later for automatic acceptance
                await services.newUserHelper.AddWelcomeMessage(message, client);
            }
        }

        private async Task HandleChannelPinsUpdated(DiscordClient client, ChannelPinsUpdateEventArgs eventArgs)
        {
            await services.pinHelper.Pin(eventArgs.Channel, client);
        }
    }
}