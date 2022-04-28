using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace ScatterBot_v2.core.Extensions
{
    public static class DiscordGuildExtensions
    {
        public static DiscordRole GetRole(this DiscordGuild guild, ulong id)
        {
            return guild.GetRole(id);
        }

        public static DiscordRole GetRole(this DiscordGuild guild, string id)
        {
            var roles = guild.Roles;
            foreach (var role in roles) {
                if (role.Value.Name == id) {
                    return role.Value;
                }
            }

            return null;
        }

        public static DiscordChannel GetChannel(this DiscordGuild guild, ulong id)
        {
            DiscordChannel channel;

            if (id == 0) {
                channel = guild.Channels.First().Value;
            }
            else {
                channel = guild.GetChannel(id);
            }

            return channel;
        }

        public static async Task<DiscordChannel> GetChannel(this DiscordGuild guild, string channelId)
        {
            DiscordChannel channel = null;

            if (channelId != "") {
                var channels = await guild.GetChannelsAsync();
                foreach (var ch in channels) {
                    if (ch.Name == channelId) {
                        channel = ch;
                        break;
                    }
                }
            }

            if (channel == null) {
                channel = guild.Channels.First().Value;
            }

            return channel;
        }

        public static async Task BotLog(this DiscordGuild guild, string msg, ulong logChannel)
        {
            var botLog = guild.GetChannel(logChannel);
            await botLog.SendMessageAsync(msg);
        }

        public static async Task LogToChannel(this DiscordGuild guild, string msg, ulong id)
        {
            var channel = guild.GetChannel(id);
            await channel.SendMessageAsync(msg);
        }
    
        public static async Task SendMsg(this DiscordGuild guild, ulong channelId, string msg)
        {
            var channel = guild.GetChannel(channelId);
            await channel.SendMessageAsync(msg);
        }
    
        public static async Task SendMsg(this DiscordGuild guild, string channelId, string msg)
        {
            var channel = await guild.GetChannel(channelId);
            await channel.SendMessageAsync(msg);
        }
    }
}