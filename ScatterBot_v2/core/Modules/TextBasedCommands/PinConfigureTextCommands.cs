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
        [Command("monitor")]
        public async Task Pin(CommandContext context, string channelId)
        {
            var channel = await context.Client.GetChannel(channelId);
            PinHelper.Instance.AddChannel(channel.Id);
            await context.Client.LogToChannel($"Monitoring channel {channel.Name}");
        }

        [Command("monitor")]
        public async Task Pin(CommandContext context, DiscordChannel channelId)
        {
            PinHelper.Instance.AddChannel(channelId.Id);
            await context.Client.LogToChannel($"Monitoring channel {channelId.Name}");
        }

        [Command("unmonitor")]
        public async Task UnPin(CommandContext context, string channelId)
        {
            var channel = await context.Client.GetChannel(channelId);
            PinHelper.Instance.RemoveChannel(channel.Id);
            await context.Client.LogToChannel($"Unmonitoring channel {channel.Name}");
        }

        [Command("unmonitor")]
        public async Task UnPin(CommandContext context, DiscordChannel channelId)
        {
            PinHelper.Instance.RemoveChannel(channelId.Id);
            await context.Client.LogToChannel($"Unmonitoring channel {channelId.Name}");
        }

        [Command("setarchive")]
        public async Task SetArchive(CommandContext context, string channelId)
        {
            var channel = await context.Client.GetChannel(channelId);
            PinHelper.Instance.SetArchiveChannel(channel.Id);
            await context.Client.LogToChannel($"{channel.Name} set as archive.");
        }

        [Command("setarchive")]
        public async Task SetArchive(CommandContext context, DiscordChannel channelId)
        {
            PinHelper.Instance.SetArchiveChannel(channelId.Id);
            await context.Client.LogToChannel($"{channelId.Name} set as archive.");
        }
    }
}