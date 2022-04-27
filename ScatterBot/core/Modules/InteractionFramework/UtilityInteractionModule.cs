using Discord.Interactions;

namespace ScatterBot.core.Modules.InteractionFramework;

public class UtilityInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("nothing", "nothing")]
    public Task Nothing()
    {
        return Task.CompletedTask;
    }
}