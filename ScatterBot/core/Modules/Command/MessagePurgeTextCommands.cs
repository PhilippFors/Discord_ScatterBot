using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Extensions;

namespace ScatterBot.core.Modules.Command;

[Group("purge")]
[RequireUserPermission(GuildPermission.ManageChannels)]
public class MessagePurgeTextCommands : ModuleBase<SocketCommandContext>
{
    [Command("channel", RunMode = RunMode.Async)]
    public async Task PurgeChannelMessages(int amount, string channelId = "")
    {
        var channel = Context.GetChannel(channelId);

        var messages = await GetMessages(amount, channel);
        var arr = messages.ToArray();

        foreach (var ele in arr) {
            await ele.DeleteAsync();
            await Task.Delay(800);
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
                await Task.Delay(800);
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
                await Task.Delay(500);
            }
        }

        if (purgeSelf) {
            await Context.Message.DeleteAsync();
        }
    }

    private async Task<IEnumerable<IMessage>> GetMessages(int amount, IMessageChannel channel)
    {
        var messages = await channel.GetMessagesAsync(amount).FlattenAsync();
        await Task.Delay(TimeSpan.FromSeconds(1));
        return messages;
    }
}