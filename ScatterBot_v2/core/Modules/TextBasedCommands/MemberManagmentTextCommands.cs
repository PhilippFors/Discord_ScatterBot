using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Helpers;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Modules.TextBasedCommands
{
    [Group("admin")]
    [RequirePermissions(Permissions.ModerateMembers)]
    public class MemberManagementTextCommands : BaseCommandModule
    {
        public BonkedHelper bonkedHelper { private get; set; }
        public SaveSystem saveSystem { private get; set; }
    
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
                foreach (var user in users) {
                    if (user.HasRole(accessRole)) {
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
                await newUserHelper.AssignRoles(context.Client);
            }

            [Command("automateaccess")]
            public async Task AutomateWelcome()
            {
                newUserHelper.StartAutomateUserWelcome();
            }

            [Command("stopautomateaccess")]
            public async Task StopAutomateWelcome()
            {
                newUserHelper.StopAutomateUserWelcome();
            }

            [Command("banusername")]
            [RequireUserPermissions(Permissions.BanMembers)]
            public async Task BanAsync(CommandContext context, DiscordMember user, int time = 0, string reason = "")
            {
                // await Context.Guild.AddBanAsync(user, time, reason);
                // await Context.Client.LogToChannel($"{user.Username} has been banned.");
            }

            [Command("banid")]
            [RequireUserPermissions(Permissions.BanMembers)]
            public async Task BanAsync(CommandContext context, ulong id, int time = 0, string reason = "")
            {
                // await Context.Guild.AddBanAsync(id, time, reason);
                // await Context.Client.LogToChannel($"{id} has been banned.");
            }
        }

        [Command("bonkid")]
        public async Task Bonk(CommandContext context, ulong id, double time = 1)
        {
            await bonkedHelper.AddBonkedMember(id, time);
        }

        [Command("bonk")]
        public async Task Bonk(CommandContext context, DiscordMember user, double time = 1)
        {
            await bonkedHelper.AddBonkedMember(user.Id, time);
        }

        [Command("unbonkid")]
        public async Task UnBonk(CommandContext context, ulong id)
        {
            await bonkedHelper.UnbonkMember(id);
        }

        [Command("unbonk")]
        public async Task UnBonk(CommandContext context, DiscordMember user)
        {
            await bonkedHelper.UnbonkMember(user.Id);
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

            await context.Client.LogToChannel($"Granted {role} to {string.Join(", ", users.Select(u => u.Username))}", saveSystem.ServerData.botLogChannel);
            await context.Message.DeleteAsync();
        }
    
        [Command("bonkamount")]
        public async Task BonkAmount(CommandContext context)
        {
            var amount = saveSystem.ServerData.bonkedMembers.Length;
            await context.Client.LogToChannel($"{amount.ToString()} people are bonked right now.", saveSystem.ServerData.botLogChannel);
        }
    }
}