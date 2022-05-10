using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;

namespace ScatterBot_v2.core.Commands
{
    [Group("purge")]
    [RequireUserPermissions(Permissions.Administrator)]
    public class MessagePurgeCommands : BaseCommandModule
    {
        [Command("channel")]
        public async Task PurgeChannelMessages(CommandContext context, int amount, string channelId = "")
        {
            DiscordChannel channel = null;
            if(channelId != "")
                channel = await context.Client.GetChannel(channelId);
            else
                channel = context.Channel;
        
            var messages = await GetMessages(context, amount + 1, channel);
            var arr = messages.ToArray();

            await Task.Delay(1000);
            foreach (var ele in arr) {
                await ele.DeleteAsync();
                await Task.Delay(800);
            }
        }

        [Command("user")]
        public async Task PurgeMessagesByUser(CommandContext context, int amount, ulong id, string channelId = "", bool purgeSelf = true)
        {
            DiscordChannel channel = null;
            if(channelId != "")
                channel = await context.Client.GetChannel(channelId);
            else
                channel = context.Channel;

            var messages = await GetMessages(context, amount, channel);
        
            await Task.Delay(1000);
            foreach (var ele in messages) {
                if (ele.Author.Id == id) {
                    await ele.DeleteAsync();
                    await Task.Delay(800);
                }
            }
        
            if (purgeSelf) {
                await context.Message.DeleteAsync();
            }
        }

        [Command("user")]
        public async Task PurgeMessagesByUser(CommandContext context, int amount, DiscordMember user, string channelId = "", bool purgeSelf = true)
        {
            DiscordChannel channel = null;
            if(channelId != "")
                channel = await context.Client.GetChannel(channelId);
            else
                channel = context.Channel;
        
            var messages = await GetMessages(context, amount, channel);

            await Task.Delay(1000);
            foreach (var ele in messages) {
                if (ele.Author.Id == user.Id) {
                    await ele.DeleteAsync();
                    await Task.Delay(800);
                }
            }
        
            if (purgeSelf) {
                await context.Message.DeleteAsync();
            }
        }

        private async Task<IEnumerable<DiscordMessage>> GetMessages(CommandContext context, int amount, DiscordChannel channel)
        {
            var messages = await channel.GetMessagesAsync(amount);
            await Task.Delay(TimeSpan.FromSeconds(1));
            return messages;
        }
    }
}