using Discord;
using Discord.Interactions;

namespace ScatterBot.core.Extensions;

public static class SocketInteractionExtensions
{
    public static IMessageChannel GetChannel(this SocketInteractionContext context, ulong id)
    {
        var channel = context.Guild.GetChannel(id);
        return channel as IMessageChannel;
    }

    public static IMessageChannel GetChannel(this SocketInteractionContext context, string channelId)
    {
        IMessageChannel channel;
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
}