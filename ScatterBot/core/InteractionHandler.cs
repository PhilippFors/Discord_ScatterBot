using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ScatterBot.core.Helpers;
using ScatterBot.core.Modules.InteractionFramework;

namespace ScatterBot.core;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services)
    {
        _client = client;
        _handler = handler;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        _client.Ready += ReadyAsync;
        _handler.Log += LogAsync;
        
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        
        _client.InteractionCreated += HandleInteraction;
    }

    private async Task LogAsync(LogMessage log)
        => Console.WriteLine(log);

    private async Task ReadyAsync()
    {
        // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
        // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
#if DEBUG
        await _handler.RegisterCommandsToGuildAsync(HardcodedShit.guildId);
#else
        await _handler.RegisterCommandsGloballyAsync(true);
#endif
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try {
            var context = new SocketInteractionContext(_client, interaction);
            
            var result = await _handler.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess) {
                switch (result.Error) {
                    case InteractionCommandError.UnmetPrecondition:
                        await interaction.RespondAsync("You don't have the permission to use this, dumbass", ephemeral: true);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        await interaction.RespondAsync("Command doesn't exist.", ephemeral: true);
                        break;
                    case InteractionCommandError.BadArgs:
                        await interaction.RespondAsync("Bad arguments, try again", ephemeral: true);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        await interaction.RespondAsync("Command unsuccessful", ephemeral: true);
                        break;
                    default:
                        await interaction.RespondAsync("Command wasn't executed", ephemeral: true);
                        break;
                }
            }
            else {
                await interaction.DeferAsync();
                await interaction.RespondAsync("Success!", ephemeral: true);
            }
        }
        catch {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }
}