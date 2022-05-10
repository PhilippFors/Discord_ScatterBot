using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.Data;

namespace ScatterBot_v2.core.Commands.ModCommands
{
    public partial class ModCommands : BaseCommandModule
    {
        [Command("ban")]
        [Description("Bans a user from the server.")]
        [RequireUserPermissions(Permissions.BanMembers)]
        public async Task BanAsync(CommandContext context, DiscordMember user, int deleteMessageDays = 0,
            [RemainingText] string reason = "ban")
        {
            await memberManagmentService.BanMember(context, user, deleteMessageDays, reason);
        }

        [Command("ban")]
        [Description("Bans a user id from the server.")]
        [RequireUserPermissions(Permissions.BanMembers)]
        public async Task BanAsync(CommandContext context, ulong id, int deleteMessageDays = 0,
            [RemainingText] string reason = "")
        {
            var user = await Guild.guild.GetMemberAsync(id);
            await memberManagmentService.BanMember(context, user, deleteMessageDays, reason);
        }

        [Command("warn")]
        [Description("Warns a user from the server.")]
        public async Task Warn(CommandContext context, DiscordMember user, bool bonk = false, double bonkTime = 1,
            [RemainingText] string reason = "")
        {
            memberManagmentService.WarnMember(context, user, reason);
            if (bonk) {
                await MuteHelperService.AddBonkedMember(user.Id, bonkTime);
            }
        }

        [Command("checkwarning")]
        [Description("Checks the warning level of a user.")]
        public Task CheckWarn(CommandContext context, DiscordMember user)
        {
            memberManagmentService.QueryWarn(context, user);
            return Task.CompletedTask;
        }

        [Command("removewarning")]
        [Description("Removes a number of warnings from a user.")]
        public Task RemoveWarn(CommandContext context, DiscordMember user, int warnings)
        {
            memberManagmentService.RemoveWarnings(context, user, warnings);
            return Task.CompletedTask;
        }

        [Command("bonk")]
        [Description("Removes all roles and assigns a muted role for a set amount of time.")]
        public async Task Bonk(CommandContext context, DiscordMember user, double time = 5)
        {
            await MuteHelperService.AddBonkedMember(user.Id, time);
        }

        [Command("bonk")]
        [Description("Removes all roles and assigns a muted role for a set amount of time.")]
        public async Task Bonk(CommandContext context, ulong id, double time = 5)
        {
            await MuteHelperService.AddBonkedMember(id, time);
        }

        [Command("unbonk")]
        [Description("Returns original roles and unmutes.")]
        public async Task UnBonk(CommandContext context, DiscordMember user)
        {
            await MuteHelperService.UnbonkMember(user.Id);
        }

        [Command("unbonk")]
        [Description("Returns original roles and unmutes.")]
        public async Task UnBonk(CommandContext context, ulong id)
        {
            await MuteHelperService.UnbonkMember(id);
        }

        [Command("addrole")]
        [Description("Adds a role to a user.")]
        public async Task GrantRole(CommandContext context, DiscordRole roleName, DiscordMember user)
        {
            await user.GrantRoleAsync(roleName);
        }

        [Command("addrole")]
        [Description("Adds a role to a user.")]
        public async Task GrantRole(CommandContext context, string roleName, DiscordMember user)
        {
            await user.GrantRoleAsync(GetRole(context, roleName));
        }

        [Command("removerole")]
        [Description("Removes a role from a user.")]
        public async Task RemoveRole(CommandContext context, DiscordRole roleName, DiscordMember user)
        {
            await user.RevokeRoleAsync(roleName);
        }

        [Command("removerole")]
        [Description("Removes a role from a user.")]
        public async Task RemoveRole(CommandContext context, string roleName, DiscordMember user)
        {
            await user.RevokeRoleAsync(GetRole(context, roleName));
        }

        [Command("addrole_bulk")]
        [Description("Adds a role to a group of users.")]
        public async Task AddRoleBulk(CommandContext context, string roleName, params DiscordMember[] users)
        {
            var role = GetRole(context, roleName);
            foreach (var user in users) {
                await user.GrantRoleAsync(role);
            }
        }

        [Command("bonkcount")]
        [Description("Returns the amount of bonked people.")]
        public async Task BonkAmount(CommandContext context)
        {
            var amount = saveSystem.ServerData.bonkedMembers.Length;
            await context.Guild.LogToChannel($"{amount.ToString()} people are bonked right now.",
                saveSystem.ServerData.botLogChannel);
        }

        private DiscordRole GetRole(CommandContext context, string roleName) => context.Guild.GetRole(roleName);
    }
}