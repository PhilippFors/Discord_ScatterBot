using Discord.Interactions;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.InteractionFramework;

public class UtilityInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("new_user_roles", "Assigns the 'possibly human' role to new users.")]
    public async Task AssignRolesToNewUser()
    {
        await RoleAssignHelper.Instance.AssignRoles(Context.Client);
    }
}