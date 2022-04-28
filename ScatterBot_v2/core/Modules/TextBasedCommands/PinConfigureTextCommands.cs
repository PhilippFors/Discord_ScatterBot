using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Helpers;

namespace ScatterBot_v2.core.Modules.TextBasedCommands
{
    [Group("pin")]
    [RequirePermissions(Permissions.ManageChannels)]
    public class PinConfigureTextCommands : BaseCommandModule
    {
        public PinHelper pinHelper { private get; set; }
        [Command("monitor")]
        public async Task Pin(CommandContext context, string channelId)
        {
            var channel = await context.Client.GetChannel(channelId);
            pinHelper.AddChannel(channel.Id);
            await context.Client.LogToChannel($"Monitoring channel {channel.Name}");
        }

        [Command("monitor")]
        public async Task Pin(CommandContext context, DiscordChannel channelId)
        {
            pinHelper.AddChannel(channelId.Id);
            await context.Client.LogToChannel($"Monitoring channel {channelId.Name}");
        }

        [Command("unmonitor")]
        public async Task UnPin(CommandContext context, string channelId)
        {
            var channel = await context.Client.GetChannel(channelId);
            pinHelper.RemoveChannel(channel.Id);
            await context.Client.LogToChannel($"Unmonitoring channel {channel.Name}");
        }

        [Command("unmonitor")]
        public async Task UnPin(CommandContext context, DiscordChannel channelId)
        {
            pinHelper.RemoveChannel(channelId.Id);
            await context.Client.LogToChannel($"Unmonitoring channel {channelId.Name}");
        }

        [Command("setarchive")]
        public async Task SetArchive(CommandContext context, string channelId)
        {
            var channel = await context.Client.GetChannel(channelId);
            pinHelper.SetArchiveChannel(channel.Id);
            await context.Client.LogToChannel($"{channel.Name} set as archive.");
        }

        [Command("setarchive")]
        public async Task SetArchive(CommandContext context, DiscordChannel channelId)
        {
            pinHelper.SetArchiveChannel(channelId.Id);
            await context.Client.LogToChannel($"{channelId.Name} set as archive.");
        }
    }
}