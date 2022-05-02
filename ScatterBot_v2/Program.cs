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
using ScatterBot_v2.core.Services;
using ScatterBot_v2.Data;
using ScatterBot_v2.Serialization;
using Serilog;
using ServiceCollection = ScatterBot_v2.core.Services.ServiceCollection;

namespace ScatterBot_v2
{
    /// <summary>
    /// Initializes the bot and various services.
    /// </summary>
    public class Program
    {
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private ServiceCollection serviceCollection;

        // Entry point
        public static void Main() =>
            new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            // get token from text file
            var stream = File.OpenRead("token.txt");
            var token = await new StreamReader(stream).ReadToEndAsync();
            await stream.DisposeAsync();

            Logging.Create();
            var logFactory = new LoggerFactory().AddSerilog();

            _client = new DiscordClient(
                new()
                {
                    Token = token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    GatewayCompressionLevel = GatewayCompressionLevel.Payload,
                    Intents = DiscordIntents.Guilds | DiscordIntents.GuildBans | DiscordIntents.GuildEmojis |
                              DiscordIntents.GuildMessages | DiscordIntents.GuildPresences | DiscordIntents.Guilds |
                              DiscordIntents.GuildMembers,
                    MessageCacheSize = 500,
                    MinimumLogLevel = LogLevel.Debug,
                    LoggerFactory = logFactory
                }
            );

            var applicationHandler = new ApplicationService();
            var saveSystem = new SaveSystem();
            saveSystem.LoadData();

            serviceCollection = new ServiceCollection()
            {
                SaveSystem = saveSystem,
                ApplicationService = applicationHandler,
                MuteHelperService = new MuteHelperService(saveSystem),
                NewUserHelperService = new NewUserHelperService(saveSystem),
                PinHelperService = new PinHelperService(saveSystem),
                MemberModerationService = new MemberModerationService(saveSystem)
            };

            await InitializeCommandHandlers();
            await _client.ConnectAsync();

            Guild.guild = await _client.GetGuildAsync(Guild.guildId);
            await serviceCollection.SaveSystem.InitializeRuntimeData(Guild.guild);

            _client.ChannelPinsUpdated += HandleChannelPinsUpdated;
            _client.MessageCreated += HandleMessagesCreated;
            _client.GuildMemberAdded += (ctx, args) => serviceCollection.NewUserHelperService.AddUser(args.Member.Id);

            // exit when program closes or token is canceled
            await Task.Delay(-1, applicationHandler.applicationTerminationToken.Token);
        }

        private Task InitializeCommandHandlers()
        {
            var di = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
                .AddSingleton(serviceCollection.MuteHelperService)
                .AddSingleton(serviceCollection.NewUserHelperService)
                .AddSingleton(serviceCollection.PinHelperService)
                .AddSingleton(serviceCollection.SaveSystem)
                .AddSingleton(serviceCollection.ApplicationService)
                .AddSingleton(serviceCollection.MemberModerationService)
                .BuildServiceProvider();

            var config = new CommandsNextConfiguration()
            {
                StringPrefixes = new[] {"!"},
                CaseSensitive = true,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = false,
                Services = di
            };

            _commands = _client.UseCommandsNext(config);
            _commands.RegisterCommands(Assembly.GetEntryAssembly());
            _commands.CommandErrored += ErrorHandler.HandleCommandErrored;

            return Task.CompletedTask;
        }

        private async Task HandleMessagesCreated(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message.Channel.Id != serviceCollection.SaveSystem.ServerData.welcomeChannel)
            {
                return;
            }

            // new user messages will be logged and can be used later for automatic acceptance
            await serviceCollection.NewUserHelperService.AddWelcomeMessage(message);

            // Bonked users can still type in welcome so this bonks 'em again
            var author = message.Author;

            if (serviceCollection.MuteHelperService.IsBonked(author.Id))
            {
                var response = await eventArgs.Message.RespondAsync($"No talking for you {author.Mention}.");
                await message.DeleteAsync();
                await response.WaitDeleteMessage(15);
            }
        }

        private async Task HandleChannelPinsUpdated(DiscordClient client, ChannelPinsUpdateEventArgs eventArgs)
        {
            await serviceCollection.PinHelperService.Pin(eventArgs.Channel, client);
        }
    }
}