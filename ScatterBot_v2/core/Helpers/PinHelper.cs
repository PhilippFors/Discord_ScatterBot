using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;

namespace ScatterBot_v2.core.Helpers;

public class PinHelper
{
    public static PinHelper Instance {
        get {
            if (instance == null) {
                instance = new PinHelper();
            }

            return instance;
        }
    }

    private static PinHelper? instance;

    private readonly List<ulong> monitoredChannels;
    private ulong archiveChannel;

    public PinHelper()
    {
        monitoredChannels = new List<ulong>();
    }

    public void AddChannel(ulong id) => monitoredChannels.Add(id);

    public void RemoveChannel(ulong id) => monitoredChannels.Remove(id);

    public void SetArchiveChannel(ulong id) => archiveChannel = id;

    public async Task Pin(DiscordChannel channel, DiscordClient client)
    {
        if (!monitoredChannels.Contains(channel.Id)) {
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
            
            var postChannel = await client.GetChannel(archiveChannel); // posting copy in here

            await postChannel.SendMessageAsync($"**Shared by:** {message.Author.Mention}\n{message.Content}");
            var webClient = new WebClient();
            
            foreach (var att in attachments) {
                using (webClient) {
                    await webClient.DownloadFileTaskAsync(att.ProxyUrl, $"{att.FileName}");
                }
                Console.WriteLine($"Downloaded {att.FileName}");
                
                await using (var file = File.Open($"{att.FileName}", FileMode.Open, FileAccess.Read)) {
                    var attachmentMsg = await new DiscordMessageBuilder().WithFile(att.FileName, file).SendAsync(postChannel);
                }
                
                File.Delete($"{att.FileName}");
            }

            await client.LogToChannel($"Archived {message.Author.Username}'s message to {postChannel.Name}");
        }
    }
}