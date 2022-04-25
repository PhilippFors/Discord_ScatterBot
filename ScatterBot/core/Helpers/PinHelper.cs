using System.Net;
using Discord;
using Discord.WebSocket;

namespace ScatterBot.core.Helpers;

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

    private List<ulong> monitoredChannels;
    private ulong archiveChannel;
    
    public PinHelper()
    {
        monitoredChannels = new List<ulong>();
    }
    
    public void AddChannel(ulong id) => monitoredChannels.Add(id);
    
    public void RemoveChannel(ulong id) => monitoredChannels.Remove(id);

    public void SetArchiveChannel(ulong id) => archiveChannel = id;
    
    public async Task Pin(SocketMessage m, ISocketMessageChannel channel, DiscordSocketClient client)
    {
        var ch = channel as IMessageChannel;
        if (!monitoredChannels.Contains(ch.Id)) {
            return;
        }

        var message = m as SocketUserMessage;
        if (message == null) {
            return;
        }

        var guild = client.GetGuild(967532421975273563);
        var audit = await guild.GetAuditLogsAsync(1, actionType: ActionType.MessagePinned).FlattenAsync();
        var auditLog = audit.FirstOrDefault();
        var guildUser = guild.GetUser(auditLog.User.Id);
        var perms = guildUser.GuildPermissions;
        
        if(!perms.Has(GuildPermission.ManageChannels))
        {
            return;
        }

        if (message.IsPinned) {
            var attachments = message.Attachments.ToList();

            await message.UnpinAsync();

            var postChannel = client.GetChannel(archiveChannel); // posting copy in here
            var messageChannel = postChannel as ISocketMessageChannel;

            await messageChannel.SendMessageAsync($"**Posted by** {m.Author.Mention}\n{m.Content}");
            var webClient = new WebClient();
            foreach (var att in attachments) {
                using (webClient) {
                    await webClient.DownloadFileTaskAsync(att.ProxyUrl, $"{att.Filename}");
                }
                await messageChannel.SendFileAsync($"{att.Filename}", isSpoiler: att.IsSpoiler());
                File.Delete($"{att.Filename}");
            }
            
            await client.LogToChannel($"Archived {m.Author.Username}'s message to {messageChannel.Name}");
        }
    }
}