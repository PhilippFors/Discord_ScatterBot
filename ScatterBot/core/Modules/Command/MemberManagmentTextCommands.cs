using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.Command;

[Group("admin")]
[RequireUserPermission(GuildPermission.ModerateMembers)]
public class MemberManagementTextCommands : ModuleBase<SocketCommandContext>
{
    [Group("access")]
    public class ServerAccess : ModuleBase<SocketCommandContext>
    {
        [Command("direct")]
        public async Task GrantAccess(SocketUser user, params IUser[] users)
        {
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
    }

    [Command("add_role_bulk")]
    public async Task AddRoleBulk(string roleName, params SocketUser[] users)
    {
        var role = Context.Guild.Roles.ToList().Find(x => x.Name == roleName);
        foreach (var user in users) {
            var guildUser = Context.Guild.GetUser(user.Id);
            await guildUser.AddRoleAsync(role);
        }
    
        await Context.Message.DeleteAsync();
    }
}