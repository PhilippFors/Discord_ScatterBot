using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Services;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Commands.ModCommands
{
    [Group("mod")]
    [RequirePermissions(Permissions.ModerateMembers)]
    public partial class ModCommands : BaseCommandModule
    {
        public MuteHelperService MuteHelperService { get; set; }
        public SaveSystem saveSystem { get; set; }
        public MemberModerationService memberManagmentService { get; set; }


        [Group("access")]
        [RequirePermissions(Permissions.ModerateMembers)]
        public class ServerAccess : BaseCommandModule
        {
            public NewUserHelperService NewUserHelperService { private get; set; }
            public SaveSystem saveSystem { private get; set; }

            [GroupCommand]
            public async Task GrantAccess(CommandContext context, params DiscordMember[] users)
            {
                var mentions = new List<string>();
                var accessRole = saveSystem.LoadAs<RoleData>().accessRoleId;
                foreach (var user in users)
                {
                    if (user.HasRole(accessRole))
                    {
                        continue;
                    }

                    NewUserHelperService.RemoveUser(user.Id);
                    mentions.Add(user.Mention);
                    await user.GrantRoleAsync(
                        context.Guild.GetRole(accessRole)
                    );
                }

                var channel = context.Guild.GetChannel(saveSystem.LoadAs<ChannelConfigs>().welcomeChannel);
                var m = string.Join(", ", mentions);
                channel.SendMessageAsync($"Hi {m}. Don't break anything.");
                await context.Message.DeleteAsync();
            }

            [Command("newusers")]
            public async Task AssignRolesToNewUser(CommandContext context)
            {
                await NewUserHelperService.AssignRoles();
            }

            [Command("automate")]
            public Task AutomateWelcome(CommandContext context)
            {
                NewUserHelperService.StartAutomateUserWelcome();
                return Task.CompletedTask;
            }

            [Command("stopautomate")]
            public Task StopAutomateWelcome(CommandContext context)
            {
                NewUserHelperService.StopAutomateUserWelcome();
                return Task.CompletedTask;
            }
        }
    }
}