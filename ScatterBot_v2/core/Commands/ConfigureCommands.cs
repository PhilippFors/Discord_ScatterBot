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
        public SaveSystem saveSytem { private get; set; }

        [Command("welcome")]
        public async Task InitWelcomeChannel(CommandContext context, ulong channelId)
        {
            saveSytem.ServerData.welcomeChannel = channelId;
            await saveSytem.SaveData();
        }

        [Command("botlog")]
        public async Task InitBotLogChannel(CommandContext context, ulong channelId)
        {
            saveSytem.ServerData.botLogChannel = channelId;
            await saveSytem.SaveData();
        }

        [Command("muted")]
        public async Task InitWelcomeChannel(CommandContext context, DiscordRole mutedRole)
        {
            saveSytem.ServerData.mutedRoleId = mutedRole.Id;
            await saveSytem.SaveData();
        }

        [Command("access")]
        public async Task InitBotLogChannel(CommandContext context, DiscordRole accessRole)
        {
            saveSytem.ServerData.accessRoleId = accessRole.Id;
            await saveSytem.SaveData();
        }

        [Command("save")]
        public Task Save(CommandContext context)
        {
            return saveSytem.SaveData();
        }
    }
}