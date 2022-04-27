using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace ScatterBot_v2.core.Extensions;

public static class DiscordClientExtensions
{
    public static async Task<DiscordChannel> GetChannel(this DiscordClient context, ulong id)
    {
        var guild = await context.GetGuildAsync(HardcodedShit.guildId);
        var channel = guild.GetChannel(id);
        return channel;
    }

    public static async Task<DiscordChannel> GetChannel(this DiscordClient context, string channelId)
    {
        var guild = await context.GetGuildAsync(HardcodedShit.guildId);

        DiscordChannel channel = null;

        if (channelId != "") {
            var c = await guild.GetChannelsAsync();
            foreach (var ch in c) {
                if (ch.Name == channelId) {
                    channel = ch;
                    break;
                }
            }
        }

        if (channel == null) {
            await context.LogToChannel($"Channel with name {channelId} not found.");
            return null;
        }

        return channel;
    }

    public static async Task<DiscordRole> GetRole(this DiscordClient client, ulong id)
    {
        var guild = await client.GetGuildAsync(HardcodedShit.guildId);
        return guild.GetRole(id);
    }

    public static async Task<DiscordRole> GetRole(this DiscordClient client, string id)
    {
        var guild = await client.GetGuildAsync(HardcodedShit.guildId);
        return guild.GetRole(id);
    }
    
    public static async Task LogToChannel(this DiscordClient context, string msg)
    {
        var botlogschannel = await context.GetChannelAsync(HardcodedShit.botLogsId);
        await botlogschannel.SendMessageAsync(msg);
    }
}