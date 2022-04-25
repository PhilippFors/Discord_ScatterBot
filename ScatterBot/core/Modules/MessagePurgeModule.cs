using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules;

[Group("purgemsg")]
[RequireUserPermission(GuildPermission.ManageChannels)]
public class MessagePurgeModule : ModuleBase<SocketCommandContext>
{
    [Command("channel")]
    public async Task PurgeChannelMessages(int amount, string channelId = "")
    {
        var channel = Context.GetChannel(channelId);

        var messages = await channel.GetMessagesAsync(amount).FlattenAsync();
        foreach (var ele in messages) {
            await ele.DeleteAsync();
            await Task.Delay(100);
        }
    }

    [Command("user")]
    public async Task PurgeMessagesByUser(int amount, ulong id, string channelId = "", bool purgeSelf = false)
    {
        IMessageChannel channel = Context.GetChannel(channelId);

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

    [Command("user")]
    public async Task PurgeMessagesByUser(int amount, SocketUser user, string channelId = "", bool purgeSelf = false)
    {
        IMessageChannel channel = Context.GetChannel(channelId);

        var messages = await GetMessages(amount, channel);
        foreach (var ele in messages) {
            if (ele.Author.Id == user.Id) {
                await ele.DeleteAsync();
                await Task.Delay(100);
            }
        }

        if (purgeSelf) {
            await Context.Message.DeleteAsync();
        }
    }

    private async Task<IEnumerable<IMessage>> GetMessages(int amount, IMessageChannel channel)
    {
        var messages = await channel.GetMessagesAsync(amount).FlattenAsync();
        return messages;
    }
}