using Discord;
using Discord.Commands;

namespace ScatterBot.core.Modules;

public class TestModule : ModuleBase<SocketCommandContext>
{
    [Command("bonk")]
    public async Task Bonk(ulong id, double time)
    {
        await BonkedModule.Instance.AddBonkedMember(id, time, Context);
    }

    [Command("unbonk")]
    public async Task UnBonk(ulong id)
    {
        await BonkedModule.Instance.UnbonkMember(id, Context);
    }

    [Command("prgChMsg")]
    public async Task PurgeChannelMessages(int amount, string channelId = "")
    {
        var channel = GetChannel(channelId);

        var messages = await channel.GetMessagesAsync(amount).FlattenAsync();
        foreach (var ele in messages) {
            await ele.DeleteAsync();
            await Task.Delay(100);
        }
    }

    [Command("prgUsrMsg")]
    public async Task PurgeMessagesByUser(int amount, ulong id, string channelId = "", bool purgeSelf = false)
    {
        IMessageChannel channel = GetChannel(channelId);

        var messages = await GetMessages(amount, channel);
        foreach (var ele in messages) {
            if (ele.Author.Id == id) {
                await ele.DeleteAsync();
                await Task.Delay(100);
            }
        }

        if (purgeSelf) {
            await Context.Message.DeleteAsync();
        }
    }

    [Command("monitorPins")]
    public async Task Pin(string channelId)
    {
        var channel = GetChannel(channelId);
        PinModule.Instance.AddChannel(channel.Id);
    }

    [Command("monitorPins")]
    public async Task Pin(ulong channelId)
    {
        var channel = GetChannel(channelId);
        PinModule.Instance.AddChannel(channel.Id);
    }

    [Command("unMonitorPins")]
    public async Task UnPin(string channelId)
    {
        var channel = GetChannel(channelId);
        PinModule.Instance.RemoveChannel(channel.Id);
    }

    [Command("unMonitorPins")]
    public async Task UnPin(ulong channelId)
    {
        var channel = GetChannel(channelId);
        PinModule.Instance.RemoveChannel(channel.Id);
    }

    private async Task<IEnumerable<IMessage>> GetMessages(int amount, IMessageChannel channel)
    {
        var messages = await channel.GetMessagesAsync(amount).FlattenAsync();
        return messages;
    }

    private IMessageChannel GetChannel(ulong id)
    {
        var channel = Context.Guild.GetChannel(id);
        return channel as IMessageChannel;
    }

    private IMessageChannel GetChannel(string channelId)
    {
        IMessageChannel channel = null;
        if (channelId != "") {
            var c = Context.Guild.Channels.Where(x => x.Name == channelId).ToList();
            if (c.Count == 0) {
                return null;
            }

            channel = c[0] as IMessageChannel;
        }
        else {
            channel = Context.Channel;
        }

        return channel;
    }
}