using Discord.Commands;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules;

public class UtilityModule : ModuleBase<SocketCommandContext>
{
    [Command("newUserRoles")]
    public async Task AssignRolesToNewUser()
    {
        await RoleAssignHelper.Instance.AssignRoles(Context.Client);
    }
}