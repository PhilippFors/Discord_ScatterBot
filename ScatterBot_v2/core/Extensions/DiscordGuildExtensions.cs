using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace ScatterBot_v2.core.Extensions;

public static class DiscordGuildExtensions
{
    public static DiscordRole GetRole(this DiscordGuild guild, ulong id)
    {
        return guild.GetRole(id);
    }

    public static DiscordRole GetRole(this DiscordGuild guild, string id)
    {
        var roles = guild.Roles;
        foreach(var role in roles)
        {
            if (role.Value.Name == id) {
                return role.Value;
            }
        }

        return null;
    }
    
    public static async Task<DiscordChannel> GetChannel(this DiscordGuild guild, ulong id)
    {
        var channel = guild.GetChannel(id);
        return channel;
    }

    public static async Task<DiscordChannel> GetChannel(this DiscordGuild guild, string channelId)
    {
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
            await guild.LogToChannel($"Channel with name {channelId} not found.");
            return null;
        }

        return channel;
    }

    public static async Task LogToChannel(this DiscordGuild guild, string msg)
    {
        var botLog = await guild.GetChannel("bot-logs");
        await botLog.SendMessageAsync(msg);
    }
}