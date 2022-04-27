using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ScatterBot.core.Extensions;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.InteractionFramework;

[Group("admin", "Admin tools")]
[RequireUserPermission(GuildPermission.ModerateMembers)]
public class MemberManagementInteractionCommands : InteractionModuleBase<SocketInteractionContext>
{
    [Group("access", "access to server")]
    [RequireUserPermission(GuildPermission.ModerateMembers)]
    public class ServerAccess : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("new_users", "Assigns the 'possibly human' role to new users.")]
        public async Task AssignRolesToNewUser()
        {
            await NewUserHelper.Instance.AssignRoles(Context.Client);
        }

        [SlashCommand("automate_access", "Enables automatic accepting of people who sent introduction in #welcome.")]
        public async Task AutomateWelcome()
        {
            NewUserHelper.Instance.StartAutomateUserWelcome();
        }

        [SlashCommand("stop_automate_access", "Disables automatic accepting of people who sent introduction in #welcome.")]
        public async Task StopAutomateWelcome()
        {
            NewUserHelper.Instance.StopAutomateUserWelcome();
        }

        [SlashCommand("ban_username", "bans user with username")]
        public async Task BanAsync(SocketUser user, int time = 0, string reason = "")
        {
            await Context.Guild.AddBanAsync(user, time, reason);
            await Context.Client.LogToChannel($"{user.Username} has been banned.");
        }

        [SlashCommand("ban_id", "bans user with id")]
        public async Task BanAsync(ulong id, int time = 0, string reason = "")
        {
            await Context.Guild.AddBanAsync(id, time, reason);
            await Context.Client.LogToChannel($"{id} has been banned.");
        }
    }

    [SlashCommand("bonk_id", "bonks user with id")]
    public async Task Bonk(ulong id, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(id, time, Context.Client);
    }

    [SlashCommand("bonk_username", "bonks user by mention")]
    public async Task Bonk(SocketUser user, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(user.Id, time, Context.Client);
    }

    [SlashCommand("unbonk_id", "unbonk user")]
    public async Task UnBonk(ulong id)
    {
        await BonkedHelper.Instance.UnbonkMember(id, Context.Client);
    }

    [SlashCommand("unbonk_user", "unbonk user")]
    public async Task UnBonk(SocketUser user)
    {
        await BonkedHelper.Instance.UnbonkMember(user.Id, Context.Client);
    }

    [SlashCommand("add_role", "adds role to user")]
    public async Task GrantRole(IRole roleName, SocketUser user)
    {
        var guildUser = Context.Guild.GetUser(user.Id);
        await guildUser.AddRoleAsync(roleName);
    }

    [SlashCommand("remove_role", "removes role from user")]
    public async Task RemoveRole(IRole roleName, SocketUser user)
    {
        var guildUser = Context.Guild.GetUser(user.Id);
        await guildUser.RemoveRoleAsync(roleName);
    }
}