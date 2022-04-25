using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.InteractionFramework;

[Group("admin", "Admin tools")]
[RequireUserPermission(GuildPermission.ModerateMembers)]
public class AdminInteraction : InteractionModuleBase<SocketInteractionContext>
{
    [Group("server_access", "access to server")]
    [RequireUserPermission(GuildPermission.ModerateMembers)]
    public class Access : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("new_users", "Assigns the 'possibly human' role to new users.")]
        public async Task AssignRolesToNewUser()
        {
            await RoleAssignHelper.Instance.AssignRoles(Context.Client);
        }

        [SlashCommand("ban_username", "bans user")]
        public async Task BanAsync(SocketUser user, string reason = "")
        {
            await Context.Guild.AddBanAsync(user, reason: reason);
            await Context.Client.LogToChannel($"{user.Username} has been banned.");
        }

        [SlashCommand("ban_id", "bans user")]
        public async Task BanAsync(ulong id, string reason = "")
        {
            await Context.Guild.AddBanAsync(id, reason: reason);
            await Context.Client.LogToChannel($"{id} has been banned.");
        }
    }

    [SlashCommand("bonk_id", "bonks user")]
    public async Task Bonk(ulong id, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(id, time, Context);
    }

    [SlashCommand("bonk_username", "bonks user")]
    public async Task Bonk(SocketUser user, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(user.Id, time, Context);
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