using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Services
{
    /// <summary>
    /// System for archiving messages from a monitored channel to a specified archiving channel.
    /// </summary>
    public class PinHelperService : ISaveable
    {
        private readonly SaveSystem saveSystem;
        private ChannelConfigs channelConfigs;

        public PinHelperService(SaveSystem saveSystem)
        {
            this.saveSystem = saveSystem;
            Load();
        }

        public void AddChannel(ulong monitor, ulong archive)
        {
            channelConfigs.monitorArchiveChannel.Add(monitor, archive);
            Save();
        }

        public void RemoveChannel(ulong monitor)
        {
            channelConfigs.monitorArchiveChannel.Remove(monitor);
            Save();
        }

        public async Task Pin(DiscordChannel channel, DiscordClient client)
        {
            if (!channelConfigs.monitorArchiveChannel.ContainsKey(channel.Id)) {
                return;
            }

            var messages = await channel.GetPinnedMessagesAsync();
            if (messages == null || messages.Count == 0) {
                return;
            }

            foreach (var message in messages) {
                var attachments = message.Attachments.ToList();


                var postChannel =
                    await client.GetChannel(channelConfigs.monitorArchiveChannel[channel.Id]); // posting copy in here

                await postChannel.SendMessageAsync($"**Shared by:** {message.Author.Mention}\n{message.Content}");
                var webClient = new WebClient();

                using (webClient) {
                    foreach (var att in attachments) {
                        await webClient.DownloadFileTaskAsync(att.ProxyUrl, $"{att.FileName}");

                        await using (var file = File.Open($"{att.FileName}", FileMode.Open, FileAccess.Read)) {
                            await new DiscordMessageBuilder().WithFile(att.FileName, file).SendAsync(postChannel);
                        }

                        File.Delete($"{att.FileName}");
                    }
                }

                await message.UnpinAsync();
                
                await client.LogToChannel($"Archived {message.Author.Username}'s message to {postChannel.Name}",
                    channelConfigs.botLogChannel);
            }
        }

        public void Save()
        {
            saveSystem.SaveAs<ChannelConfigs>(channelConfigs);
        }

        public void Load()
        {
            channelConfigs = saveSystem.LoadAs<ChannelConfigs>();

            channelConfigs.monitorArchiveChannel ??= new Dictionary<ulong, ulong>();
        }
    }
}