using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Serialization;

namespace ScatterBot_v2.core.Helpers;

public class PinHelper
{
    private SaveSystem saveSystem;
    private Dictionary<ulong, ulong> monitorArchive => saveSystem.ServerData.monitorArchiveChannel;

    public PinHelper(SaveSystem saveSystem)
    {
        this.saveSystem = saveSystem;

        if (monitorArchive == null) {
            saveSystem.ServerData.monitorArchiveChannel = new Dictionary<ulong, ulong>();
        }
    }

    public void AddChannel(ulong monitor, ulong archive)
    {
        monitorArchive.Add(monitor, archive);
        saveSystem.SaveData();
    }

    public void RemoveChannel(ulong monitor)
    {
        monitorArchive.Remove(monitor);
        saveSystem.SaveData();
    }

    public async Task Pin(DiscordChannel channel, DiscordClient client)
    {
        if (!monitorArchive.ContainsKey(channel.Id)) {
            return;
        }

        var messages = await channel.GetPinnedMessagesAsync();
        var message = messages.FirstOrDefault();
        if (message == null) {
            return;
        }

        if (message.Pinned) {
            var attachments = message.Attachments.ToList();

            await message.UnpinAsync();

            var postChannel = await client.GetChannel(monitorArchive[channel.Id]); // posting copy in here

            await postChannel.SendMessageAsync($"**Shared by:** {message.Author.Mention}\n{message.Content}");
            var webClient = new WebClient();

            foreach (var att in attachments) {
                using (webClient) {
                    await webClient.DownloadFileTaskAsync(att.ProxyUrl, $"{att.FileName}");
                }

                Console.WriteLine($"Downloaded {att.FileName}");

                await using (var file = File.Open($"{att.FileName}", FileMode.Open, FileAccess.Read)) {
                    await new DiscordMessageBuilder().WithFile(att.FileName, file)
                        .SendAsync(postChannel);
                }

                File.Delete($"{att.FileName}");
            }

            await client.LogToChannel($"Archived {message.Author.Username}'s message to {postChannel.Name}",
                saveSystem.ServerData.botLogChannel);
        }
    }
}