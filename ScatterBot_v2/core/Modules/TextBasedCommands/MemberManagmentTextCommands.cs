using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Helpers;

namespace ScatterBot_v2.core.Modules.TextBasedCommands;

[Group("admin")]
[RequirePermissions(Permissions.ModerateMembers)]
public class MemberManagementTextCommands : BaseCommandModule
{
    [Group("access")]
    [RequirePermissions(Permissions.ModerateMembers)]
    public class ServerAccess : BaseCommandModule
    {
        [Command("direct")]
        public async Task GrantAccess(CommandContext context, params DiscordMember[] users)
        {
            var mentions = new List<string>();
            foreach (var user in users) {
                if (!NewUserHelper.Instance.HasUser(user)) {
                    return;
                }

                NewUserHelper.Instance.RemoveUser(user, context.Guild);
                mentions.Add(user.Mention);
                await user.GrantRoleAsync(
                    await context.Client.GetRole(HardcodedShit.humanRoleId)
                );
            }

            await context.Message.DeleteAsync();
            var channel = await context.Client.GetChannel("welcome");
            await channel.SendMessageAsync($"Hi {string.Join(", ", mentions)}. Don't break anything.");
        }

        [Command("newusers")]
        public async Task AssignRolesToNewUser(CommandContext context)
        {
            await NewUserHelper.Instance.AssignRoles(context.Client);
        }

        [Command("automateaccess")]
        public async Task AutomateWelcome()
        {
            NewUserHelper.Instance.StartAutomateUserWelcome();
        }

        [Command("stopautomateaccess")]
        public async Task StopAutomateWelcome()
        {
            NewUserHelper.Instance.StopAutomateUserWelcome();
        }

        [Command("banusername")]
        public async Task BanAsync(CommandContext context, DiscordMember user, int time = 0, string reason = "")
        {
            // await Context.Guild.AddBanAsync(user, time, reason);
            // await Context.Client.LogToChannel($"{user.Username} has been banned.");
        }

        [Command("banid")]
        public async Task BanAsync(CommandContext context, ulong id, int time = 0, string reason = "")
        {
            // await Context.Guild.AddBanAsync(id, time, reason);
            // await Context.Client.LogToChannel($"{id} has been banned.");
        }
    }

    [Command("bonkid")]
    public async Task Bonk(CommandContext context, ulong id, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(id, time, context.Client);
    }

    [Command("bonkusername")]
    public async Task Bonk(CommandContext context, DiscordMember user, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(user.Id, time, context.Client);
    }

    [Command("unbonkid")]
    public async Task UnBonk(CommandContext context, ulong id)
    {
        await BonkedHelper.Instance.UnbonkMember(id, context.Client);
    }

    [Command("unbonkuser")]
    public async Task UnBonk(CommandContext context, DiscordMember user)
    {
        await BonkedHelper.Instance.UnbonkMember(user.Id, context.Client);
    }

    [Command("addrole")]
    public async Task GrantRole(CommandContext context, DiscordRole roleName, DiscordMember user)
    {
        await user.GrantRoleAsync(roleName);
    }

    [Command("removerole")]
    public async Task RemoveRole(CommandContext context, DiscordRole roleName, DiscordMember user)
    {
        await user.RevokeRoleAsync(roleName);
    }

    [Command("addrolebulk")]
    public async Task AddRoleBulk(CommandContext context, string roleName, params DiscordMember[] users)
    {
        var role = context.Guild.GetRole(roleName);
        foreach (var user in users) {
            await user.GrantRoleAsync(role);
        }

        await context.Client.LogToChannel($"Granted {role} to {string.Join(", ", users.Select(u => u.Username))}");
        await context.Message.DeleteAsync();
    }
}