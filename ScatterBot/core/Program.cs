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
        private Logger logger;
        
        public static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            logger = new Logger();
            await logger.CreateFile();
            
            var config = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages,
                MessageCacheSize = 400,
            };

            client = new DiscordSocketClient(config);
            var f = File.OpenRead("token.txt");
            var token = await new StreamReader(f).ReadToEndAsync();
            await f.DisposeAsync();
            
            client.Log += logger.LogToConsole;
            client.Log += logger.LogToFile;
            
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
           
            // TODO: Setup dependency injection
            
            service = new CommandService();
            commandHandler = new CommandHandler(client, service);

            interaction = new InteractionService(client.Rest);
            interactionHandler = new InteractionHandler(client, interaction, null);

            await commandHandler.InstallCommandsAsync();
            await interactionHandler.InitializeAsync();

            await Task.Delay(-1);
        }


    }
}