using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ScatterBot_v2.Data;

namespace ScatterBot_v2.core.Extensions
{
    public static class DiscordClientExtensions
    {
        public static async Task<DiscordChannel> GetChannel(this DiscordClient context, ulong id)
        {
            DiscordChannel channel;
            var guild = Guild.guild;

            if (id == 0) {
                channel = guild.Channels.First().Value;
            }
            else {
                channel = guild.GetChannel(id);
            }

            return channel;
        }

        public static async Task<DiscordChannel> GetChannel(this DiscordClient context, string channelId)
        {
            var guild = Guild.guild;

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

            return channel;
        }

        public static DiscordRole GetRole(this DiscordClient client, ulong id)
        {
            var guild = Guild.guild;
            return guild.GetRole(id);
        }

        public static DiscordRole GetRole(this DiscordClient client, string id)
        {
            var guild = Guild.guild;
            return guild.GetRole(id);
        }

        public static async Task LogToChannel(this DiscordClient context, string msg, ulong id)
        {
            var botLogsChannel = await context.GetChannel(id);
            await botLogsChannel.SendMessageAsync(msg);
        }
    }
}