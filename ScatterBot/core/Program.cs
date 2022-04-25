using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ScatterBot.core
{
    public class Program
    {
        private DiscordSocketClient client;
        private CommandHandler handler;
        private CommandService service;

        public static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var config = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 400,
            };
           
            
            client = new DiscordSocketClient(config);
            var f = File.OpenRead("token.txt");
            var token = new StreamReader(f).ReadToEnd();

            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
           
            service = new CommandService();
            handler = new CommandHandler(client, service);
            await handler.InstallCommandsAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}