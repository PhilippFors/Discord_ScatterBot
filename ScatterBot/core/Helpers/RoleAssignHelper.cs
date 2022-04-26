using Discord;
using Discord.WebSocket;

namespace ScatterBot.core.Helpers;

public class RoleAssignHelper
{
    public static RoleAssignHelper Instance {
        get {
            if (instance == null) {
                instance = new RoleAssignHelper();
            }

            return instance;
        }
    }

    private static RoleAssignHelper? instance;

    private List<SocketGuildUser> newUsers;

    private string WelcomeMessageStart => "Hello and welcome";

    private string WelcomeMessageEnd => "\nYou got access now. Follow rules and use brain please.";

    private int minMessageCount = 20;

    public RoleAssignHelper()
    {
        newUsers = new List<SocketGuildUser>();
    }

    public Task AddUser(ulong id, SocketGuild guild)
    {
        var user = guild.GetUser(id);
        newUsers.Add(user);
        return Task.CompletedTask;
    }

    public void RemoveUser(SocketGuildUser user, SocketGuild guild)
    {
        newUsers.Remove(user);
    }

    public bool HasUser(SocketUser user) => newUsers.Contains(user);
    
    public async Task AssignRoles(IDiscordClient context)
    {
        if (newUsers.Count == 0) {
            return;
        }
        
        var mentionedUsers = new List<string>();
        var shortIntroList = new List<string>();
        var welcomeChannel = await context.GetChannel(HardcodedShit.welcomeId);
        var messages = await welcomeChannel.GetMessagesAsync(50).FlattenAsync();
        var messageArr = messages.ToArray();
        
        for (int i = 0; i < newUsers.Count; i++) {
            var user = newUsers[i];
            foreach (var m in messageArr) {
                if (m == null || m.Author.Id != user.Id) {
                    continue;
                }
                
                mentionedUsers.Add(user.Mention);
                newUsers.Remove(user);
                if (m.Content.Length < minMessageCount) {
                    shortIntroList.Add(user.Mention);
                }
                await user.AddRoleAsync(HardcodedShit.humanRoleId);
                break;
            }
        }
        
        if(mentionedUsers.Count == 0) {
            return;
        }
        
        var guild = await context.GetGuildAsync(HardcodedShit.guildId);
        var phil = await guild.GetUserAsync(HardcodedShit.phil);
        var message = $"{WelcomeMessageStart} {string.Join(", ", mentionedUsers)}. {WelcomeMessageEnd} {phil.Mention}";
        
        await welcomeChannel.SendMessageAsync(message);
        
        if (shortIntroList.Count > 0) {
            var addendum = $"\n{string.Join(", ", shortIntroList)}, your intros seem a little short, maybe add a bit more? You have been granted access regardless!";
            await welcomeChannel.SendMessageAsync(addendum);
        }
    }
}