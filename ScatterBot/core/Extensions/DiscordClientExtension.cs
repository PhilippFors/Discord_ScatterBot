using Discord;

namespace ScatterBot.core.Extensions;

public static class DiscordClientExtension
{
    public static async Task<IMessageChannel> GetChannel(this IDiscordClient context, ulong id)
    {
        var guild = await context.GetGuildAsync(HardcodedShit.guildId);
        var channel = await guild.GetChannelAsync(id);
        return channel as IMessageChannel;
    }

    public static async Task<IMessageChannel> GetChannel(this IDiscordClient context, string channelId)
    {
        var guild = await context.GetGuildAsync(HardcodedShit.guildId);
        IMessageChannel channel = null;
        if (channelId != "") {
            var c = guild.GetChannelsAsync().Result.Where(x => x.Name == channelId).ToList();
            if (c.Count == 0) {
                return null;
            }

            channel = c[0] as IMessageChannel;
        }

        return channel;
    }
}