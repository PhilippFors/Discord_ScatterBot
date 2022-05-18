using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Commands
{
    [Group("init")]
    [RequireUserPermissions(Permissions.Administrator)]
    public class ConfigureCommands : BaseCommandModule
    {
        public SaveSystem saveSytem { get; set; }
        
        [Command("welcome")]
        public Task InitWelcomeChannel(CommandContext context, ulong channelId)
        {
            var config = saveSytem.LoadAs<ChannelConfigs>();
            config.welcomeChannel = channelId;
            saveSytem.SaveAs<ChannelConfigs>(config);
            return Task.CompletedTask;
        }

        [Command("botlog")]
        public Task InitBotLogChannel(CommandContext context, ulong channelId)
        {
            var config = saveSytem.LoadAs<ChannelConfigs>();
            config.botLogChannel = channelId;
            saveSytem.SaveAs<ChannelConfigs>(config);
            return Task.CompletedTask;
        }

        [Command("muted")]
        public Task InitWelcomeChannel(CommandContext context, DiscordRole mutedRole)
        {
            var roles = saveSytem.LoadAs<RoleData>();
            roles.mutedRoleId = mutedRole.Id;
            saveSytem.SaveAs<RoleData>(roles);
            return Task.CompletedTask;
        }

        [Command("access")]
        public Task InitBotLogChannel(CommandContext context, DiscordRole accessRole)
        {
            var roles = saveSytem.LoadAs<RoleData>();
            roles.accessRoleId = accessRole.Id;
            saveSytem.SaveAs<RoleData>(roles);
            return Task.CompletedTask;
        }
    }
}