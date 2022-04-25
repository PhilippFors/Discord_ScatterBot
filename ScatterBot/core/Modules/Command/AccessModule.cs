using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.Command;

[Group("set")]
[RequireUserPermission(GuildPermission.ModerateMembers)]
public class AccessModule : ModuleBase<SocketCommandContext>
{
    [Command("new_users")]
    public async Task AssignRolesToNewUser()
    {
        await RoleAssignHelper.Instance.AssignRoles(Context.Client);
    }

    [Command("direct")]
    public async Task GrantAccess(SocketUser user, params IUser[] users)
    {
        Console.WriteLine("yuh");

        if (!RoleAssignHelper.Instance.HasUser(user)) {
            return;
        }

        var mentions = new List<string>();
        var guildUser = Context.Guild.GetUser(user.Id);
        mentions.Add(guildUser.Mention);
        await guildUser.AddRoleAsync(HardcodedShit.humanRoleId);
        RoleAssignHelper.Instance.RemoveUser(guildUser, Context.Guild);
        Console.WriteLine("hllo");

        foreach (var u in users) {
            guildUser = Context.Guild.GetUser(u.Id);
            RoleAssignHelper.Instance.RemoveUser(guildUser, Context.Guild);
            mentions.Add(guildUser.Mention);
            await guildUser.AddRoleAsync(HardcodedShit.humanRoleId);
        }

        await Context.Message.DeleteAsync();
        await Context.GetChannel("welcome")
            .SendMessageAsync($"Welcome to the server {string.Join(", ", mentions)}!");
    }

    [Command("add_role")]
    public async Task GrantRole(SocketUser user, IRole roleName)
    {
        Console.WriteLine("yuh");
        var guildUser = Context.Guild.GetUser(user.Id);
        await guildUser.AddRoleAsync(roleName);
        await Context.Message.DeleteAsync();
    }

    [Command("remove_role")]
    public async Task RemoveRole(SocketUser user, IRole roleName)
    {
        Console.WriteLine("yuh");
        var guildUser = Context.Guild.GetUser(user.Id);
        await guildUser.RemoveRoleAsync(roleName);
        await Context.Message.DeleteAsync();
    }
}