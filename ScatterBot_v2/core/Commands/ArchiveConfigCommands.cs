using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Services;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Commands
{
    [Group("pin")]
    [RequirePermissions(Permissions.ManageChannels)]
    public class ArchiveConfigCommands : BaseCommandModule
    {
        public PinHelperService PinHelperService { private get; set; }
        public SaveSystem saveSystem { private get; set; }
        
        [Command("monitor")]
        public async Task Pin(CommandContext context, string monitorId, string archiveId)
        {
            var monitor = await context.Client.GetChannel(monitorId);
            var archive = await context.Client.GetChannel(archiveId);
            PinHelperService.AddChannel(monitor.Id, archive.Id);
            await Log(context,$"Archiving {monitor.Name} in {archive.Name}");
        }

        [Command("monitor")]
        public async Task Pin(CommandContext context, ulong monitorId, ulong archiveId)
        {
            var monitor = await context.Client.GetChannel(monitorId);
            var archive = await context.Client.GetChannel(archiveId);
            
            if(monitor == null || archive == null)
            {
                await Log(context, "One of the channels doesn't exist");
                return;
            }
            
            PinHelperService.AddChannel(monitorId, archiveId);
            await Log(context,$"Archiving {monitor.Name} in {archive.Name}");
        }

        [Command("unmonitor")]
        public async Task UnPin(CommandContext context, string channelId)
        {
            var channel = await context.Client.GetChannel(channelId);
            PinHelperService.RemoveChannel(channel.Id);
            await Log(context,$"Unmonitoring channel {channel.Name}");
        }

        [Command("unmonitor")]
        public async Task UnPin(CommandContext context, DiscordChannel channelId)
        {
            PinHelperService.RemoveChannel(channelId.Id);
            await Log(context,$"Unmonitoring channel {channelId.Name}");
        }

        private async Task Log(CommandContext context, string msg)
        {
            await context.Client.LogToChannel(msg, saveSystem.ServerData.botLogChannel);
        }
    }
}