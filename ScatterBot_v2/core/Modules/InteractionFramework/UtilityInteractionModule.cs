using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;

namespace ScatterBot_v2.core.Modules.InteractionFramework;

public class UtilityInteractionModule : BaseCommandModule
{
    [SlashCommand("nothing", "nothing")]
    public Task Nothing()
    {
        return Task.CompletedTask;
    }
}