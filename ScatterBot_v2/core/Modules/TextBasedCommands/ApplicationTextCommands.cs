using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ScatterBot_v2.core.Helpers;

namespace ScatterBot_v2.core.Modules.TextBasedCommands;

[Group("application")]
[RequireUserPermissions(Permissions.Administrator)]
public class ApplicationTextCommands : BaseCommandModule
{
    public ApplicationHandler handler { private get; set; }

    [Command("terminate")]
    public Task Terminate(CommandContext context)
    {
        handler.Cancel();
        return Task.CompletedTask;
    }
}