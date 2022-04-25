using System.Diagnostics;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.InteractionFramework;

// [Group("grant", "access stuff")]
public class UtilityInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("nothing", "nothing")]
    public Task Nothing()
    {
        return Task.CompletedTask;
    }
}