using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Extensions;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.TextBasedCommands;

[Group("admin")]
[RequireUserPermission(GuildPermission.ModerateMembers)]
public class MemberManagementTextCommands : ModuleBase<SocketCommandContext>
{
    [Group("access")]
    [RequireUserPermission(GuildPermission.ModerateMembers)]
    public class ServerAccess : ModuleBase<SocketCommandContext>
    {
        [Command("direct")]
        public async Task GrantAccess(SocketUser user, params IUser[] users)
        {
            if (!NewUserHelper.Instance.HasUser(user)) {
                return;
            }

            var mentions = new List<string>();
            var guildUser = Context.Guild.GetUser(user.Id);
            mentions.Add(guildUser.Mention);
            await guildUser.AddRoleAsync(HardcodedShit.humanRoleId);
            NewUserHelper.Instance.RemoveUser(guildUser, Context.Guild);
            Console.WriteLine("hllo");

            foreach (var u in users) {
                guildUser = Context.Guild.GetUser(u.Id);
                NewUserHelper.Instance.RemoveUser(guildUser, Context.Guild);
                mentions.Add(guildUser.Mention);
                await guildUser.AddRoleAsync(HardcodedShit.humanRoleId);
            }

            await Context.Message.DeleteAsync();
            await Context.GetChannel("welcome").SendMessageAsync($"Hi {string.Join(", ", mentions)}. Don't break anything.");
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