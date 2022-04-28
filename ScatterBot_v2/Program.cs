using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProtoBuf.Meta;
using ScatterBot_v2.core;
using ScatterBot_v2.core.Data;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Helpers;
using ScatterBot_v2.core.Serialization;

namespace ScatterBot_v2
{
    public class Program
    {
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private Services services;

        // Entry point
        public static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            var runtime = RuntimeTypeModel.Default;
            
            // get token from text file
            var stream = File.OpenRead("token.txt");
            var token = await new StreamReader(stream).ReadToEndAsync();
            await stream.DisposeAsync();

            _client = new DiscordClient(
                new() {
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

            await InitializeHandlers();
            await _client.ConnectAsync();
            var guild = await _client.GetGuildAsync(Guild.guildId);
            
            services.saveSystem.LoadData();
            await services.saveSystem.InitializeRuntimeData(guild);
            services.bonkedHelper.Initialize(_client, services.saveSystem);

            _client.ChannelPinsUpdated += HandleChannelPinsUpdated;
            _client.MessageCreated += HandleMessagesCreated;
            _client.GuildMemberAdded += (ctx, args) => services.newUserHelper.AddUser(args.Member.Id);
            
            await Task.Delay(-1);
        }

        private Task InitializeHandlers()
        {
            services = new Services() {
                bonkedHelper = new BonkedHelper(),
                newUserHelper = new NewUserHelper(),
                pinHelper = new PinHelper(),
                saveSystem = new SaveSystem()
            };

            var di = new ServiceCollection()
                .AddSingleton(services.bonkedHelper)
                .AddSingleton(services.newUserHelper)
                .AddSingleton(services.pinHelper)
                .AddSingleton(services.saveSystem)
                .BuildServiceProvider();

            var config = new CommandsNextConfiguration() {
                StringPrefixes = new[] {"!"},
                CaseSensitive = true,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = false,
                Services = di
            };

            _commands = _client.UseCommandsNext(config);
            _commands.RegisterCommands(Assembly.GetEntryAssembly());
            _commands.CommandErrored += HandleCommandErrored;

            return Task.CompletedTask;
        }

        private async Task HandleCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            var commandName = e.Command.Name;
            var member = e.Context.Member;
            await _client.LogToChannel($"{commandName} executed by {member.Username} failed to execute.");
        }

        private async Task HandleMessagesCreated(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message.Channel.Id == Channels.welcomeChannelId) {
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