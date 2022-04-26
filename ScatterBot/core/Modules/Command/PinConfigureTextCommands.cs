using Discord;
using Discord.Commands;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.Command;

[Group("pin")]
[RequireUserPermission(GuildPermission.ManageChannels)]
public class PinConfigureTextCommands : ModuleBase<SocketCommandContext>
{
    [Command("monitor")]
    public async Task Pin(string channelId)
    {
        var channel = Context.GetChannel(channelId);
        PinHelper.Instance.AddChannel(channel.Id);
        await Context.LogToChannel($"Monitoring channel {channel.Name}");
    }

    [Command("monitor")]
    public async Task Pin(ulong channelId)
    {
        PinHelper.Instance.AddChannel(channelId);
        await Context.LogToChannel($"Monitoring channel {channelId}");
    }

    [Command("unmonitor")]
    public async Task UnPin(string channelId)
    {
        var channel = Context.GetChannel(channelId);
        PinHelper.Instance.RemoveChannel(channel.Id);
        await Context.LogToChannel($"Unmonitoring channel {channel.Name}");
    }

    [Command("unmonitor")]
    public async Task UnPin(ulong channelId)
    {
        PinHelper.Instance.RemoveChannel(channelId);
        await Context.LogToChannel($"Unmonitoring channel {channelId}");
    }

    [Command("setarchive")]
    public async Task SetArchive(string channelId)
    {
        var channel = Context.GetChannel(channelId);
        PinHelper.Instance.SetArchiveChannel(channel.Id);
        await Context.LogToChannel($"{channel.Name} set as archive.");
    }    
    
    [Command("setarchive")]
    public async Task SetArchive(ulong channelId)
    {
        PinHelper.Instance.SetArchiveChannel(channelId);
        await Context.LogToChannel($"{channelId} set as archive.");
    }
}