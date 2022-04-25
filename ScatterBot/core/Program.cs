using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace ScatterBot.core
{
    public class Program
    {
        private DiscordSocketClient client;
        private CommandHandler commandHandler;
        private CommandService service;
        private InteractionService interaction;
        private InteractionHandler interactionHandler;
        public static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var config = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages,
                MessageCacheSize = 400,
            };

            client = new DiscordSocketClient(config);
            var f = File.OpenRead("token.txt");
            var token = new StreamReader(f).ReadToEnd();
            f.Dispose();
            
            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
           
            service = new CommandService();
            commandHandler = new CommandHandler(client, service);

            interaction = new InteractionService(client.Rest);
            interactionHandler = new InteractionHandler(client, interaction, null);

            await commandHandler.InstallCommandsAsync();
            await interactionHandler.InitializeAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}