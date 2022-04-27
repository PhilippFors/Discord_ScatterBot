using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using ScatterBot_v2.core;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Helpers;

namespace ScatterBot_v2
{
    public class Program
    {
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        
        // Entry point
        public static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            // get token from text file
            var f = File.OpenRead("token.txt");
            var token = await new StreamReader(f).ReadToEndAsync();
            await f.DisposeAsync();

            // Log.Logger = new LoggerConfiguration().WriteTo
            
            _client = new DiscordClient(
                new() {
                    Token = token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    GatewayCompressionLevel = GatewayCompressionLevel.Payload,
                    Intents = DiscordIntents.Guilds | DiscordIntents.GuildBans | DiscordIntents.GuildEmojis |
                              DiscordIntents.GuildMessages | DiscordIntents.GuildPresences | DiscordIntents.Guilds,
                    MessageCacheSize = 300,
                    MinimumLogLevel = LogLevel.Debug
                }
            );

            await InitializeHandlers();
            await _client.ConnectAsync();
            _client.ChannelPinsUpdated += HandleChannelPinsUpdated;
            _client.MessageCreated += HandleMessagesCreated;
            await Task.Delay(-1);
        }

        private async Task InitializeHandlers()
        {
            var config = new CommandsNextConfiguration() {
                StringPrefixes = new[] {"!"},
                CaseSensitive = true,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = false
            };

            _commands = _client.UseCommandsNext(config);
            _commands.RegisterCommands(Assembly.GetEntryAssembly());
            _commands.CommandErrored += HandleCommandErrored;
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
            if (message.Channel.Id == HardcodedShit.welcomeId) {
                // Bonked users can still type in welcome so this bonks 'em again
                var author = message.Author;
                if (BonkedHelper.Instance.IsBonked(author.Id)) {
                    var response = await message.Channel.SendMessageAsync($"No talking for you {author.Mention}.");
                    await message.DeleteAsync();
                    await response.WaitDeleteMessage(15);
                }
        
                // new user messages will be logged and can be used later for automatic acceptance
                NewUserHelper.Instance.AddWelcomeMessage(message, client);
            }
        }
        
        private async Task HandleChannelPinsUpdated(DiscordClient client, ChannelPinsUpdateEventArgs eventArgs)
        {
            await PinHelper.Instance.Pin(eventArgs.Channel, client);
        }
    }
}