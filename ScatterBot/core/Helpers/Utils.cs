using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ScatterBot.core.Helpers;

public static class Utils
{
    public static IMessageChannel GetChannel(this SocketCommandContext context, ulong id)
    {
        var channel = context.Guild.GetChannel(id);
        return channel as IMessageChannel;
    }
    
    public static IMessageChannel GetChannel(this SocketCommandContext context, string channelId)
    {
        IMessageChannel channel = null;
        if (channelId != "") {
            var c = context.Guild.Channels.Where(x => x.Name == channelId).ToList();
            if (c.Count == 0) {
                return null;
            }

            channel = c[0] as IMessageChannel;
        }
        else {
            channel = context.Channel;
        }

        return channel;
    }

    public static async Task LogToChannel(this SocketCommandContext context, string msg)
    {
        var botlogschannel = context.GetChannel(HardcodedShit.botLogsId);
        await botlogschannel.SendMessageAsync(msg);
    }

    public static async Task LogToChannel(this DiscordSocketClient context, string msg)
    {
        var botlogschannel = context.GetChannel(HardcodedShit.botLogsId);
        var channel = botlogschannel as IMessageChannel;
        await channel.SendMessageAsync(msg);
    }
}