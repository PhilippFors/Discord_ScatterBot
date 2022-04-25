using System.Net;
using Discord;
using Discord.WebSocket;

namespace ScatterBot.core.Modules;

public class PinModule
{
    public static PinModule Instance {
        get {
            if (instance == null) {
                instance = new PinModule();
            }

            return instance;
        }
    }

    private static PinModule? instance;

    private List<ulong> monitoredChannels = new List<ulong>();

    public void AddChannel(ulong id) => monitoredChannels.Add(id);
    
    public void RemoveChannel(ulong id) => monitoredChannels.Remove(id);
    
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

            var postChannel = client.GetChannel(967532421975273566); // posting copy in here
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
        }
    }
}