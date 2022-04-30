using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Helpers;
using ScatterBot_v2.Data;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Modules.TextBasedCommands
{
    [Group("admin")]
    [RequirePermissions(Permissions.ModerateMembers)]
    public class MemberManagementTextCommands : BaseCommandModule
    {
        public BonkedHelper bonkedHelper { private get; set; }
        public SaveSystem saveSystem { private get; set; }
        public MemberManagementService memberManagmentService { private get; set; }


        [Group("access")]
        [RequirePermissions(Permissions.ModerateMembers)]
        public class ServerAccess : BaseCommandModule
        {
            public NewUserHelper newUserHelper { private get; set; }
            public SaveSystem saveSystem { private get; set; }

            [Command("direct")]
            public async Task GrantAccess(CommandContext context, params DiscordMember[] users)
            {
                var mentions = new List<string>();
                var accessRole = saveSystem.ServerData.accessRoleId;
                foreach (var user in users)
                {
                    if (user.HasRole(accessRole))
                    {
                        continue;
                    }

                    newUserHelper.RemoveUser(user.Id);
                    mentions.Add(user.Mention);
                    await user.GrantRoleAsync(
                        context.Guild.GetRole(accessRole)
                    );
                }

                var channel = context.Guild.GetChannel(saveSystem.ServerData.welcomeChannel);
                var m = string.Join(", ", mentions);
                channel.SendMessageAsync($"Hi {m}. Don't break anything.");
                await context.Message.DeleteAsync();
            }

            [Command("newusers")]
            public async Task AssignRolesToNewUser(CommandContext context)
            {
                await newUserHelper.AssignRoles();
            }

            [Command("automateaccess")]
            public Task AutomateWelcome(CommandContext context)
            {
                newUserHelper.StartAutomateUserWelcome();
                return Task.CompletedTask;
            }

            [Command("stopautomateaccess")]
            public Task StopAutomateWelcome(CommandContext context)
            {
                newUserHelper.StopAutomateUserWelcome();
                return Task.CompletedTask;
            }
        }

        [Command("ban")]
        [RequireUserPermissions(Permissions.BanMembers)]
        public async Task BanAsync(CommandContext context, DiscordMember user, int deleteMessageDays = 0,
            [RemainingText] string reason = "ban")
        {
            await memberManagmentService.BanMember(context, user, deleteMessageDays, reason);
        }

        [Command("ban_id")]
        [RequireUserPermissions(Permissions.BanMembers)]
        public async Task BanAsync(CommandContext context, ulong id, int deleteMessageDays = 0,
            [RemainingText] string reason = "")
        {
            var user = await Guild.guild.GetMemberAsync(id);
            await memberManagmentService.BanMember(context, user, deleteMessageDays, reason);
        }

        [Command("warn")]
        public async Task Warn(CommandContext context, DiscordMember user, bool bonk = false, double bonkTime = 1, [RemainingText] string reason = "")
        {
            memberManagmentService.WarnMember(context, user, reason);
            if (bonk)
            {
                await bonkedHelper.AddBonkedMember(user.Id, bonkTime);
            }
        }

        [Command("checkwarning")]
        public Task CheckWarn(CommandContext context, DiscordMember user)
        {
            memberManagmentService.QueryWarn(context, user);
            return Task.CompletedTask;
        }

        [Command("removewarning")]
        public Task RemoveWarn(CommandContext context, DiscordMember user, int warnings)
        {
            memberManagmentService.RemoveWarnings(context, user, warnings);
            return Task.CompletedTask;
        }

        [Command("bonk")]
        public async Task Bonk(CommandContext context, DiscordMember user, double time = 1)
        {
            await bonkedHelper.AddBonkedMember(user.Id, time);
        }

        [Command("bonk_id")]
        public async Task Bonk(CommandContext context, ulong id, double time = 1)
        {
            await bonkedHelper.AddBonkedMember(id, time);
        }

        [Command("unbonk")]
        public async Task UnBonk(CommandContext context, DiscordMember user)
        {
            await bonkedHelper.UnbonkMember(user.Id);
        }

        [Command("unbonkid")]
        public async Task UnBonk(CommandContext context, ulong id)
        {
            await bonkedHelper.UnbonkMember(id);
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

        [Command("addrole_bulk")]
        public async Task AddRoleBulk(CommandContext context, string roleName, params DiscordMember[] users)
        {
            var role = context.Guild.GetRole(roleName);
            foreach (var user in users)
            {
                await user.GrantRoleAsync(role);
            }

            await context.RespondAsync($"Granted {role} to {string.Join(", ", users.Select(u => u.Username))}");
        }

        [Command("bonkamount")]
        public async Task BonkAmount(CommandContext context)
        {
            var amount = saveSystem.ServerData.bonkedMembers.Length;
            await context.Guild.LogToChannel($"{amount.ToString()} people are bonked right now.",
                saveSystem.ServerData.botLogChannel);
        }
    }
}