using Discord;
using Discord.WebSocket;
using ScatterBot.core.Extensions;

namespace ScatterBot.core.Helpers;

public class NewUserHelper
{
    public static NewUserHelper Instance {
        get {
            if (instance == null) {
                instance = new NewUserHelper();
            }

            return instance;
        }
    }

    public bool automateUserWelcome;

    private static NewUserHelper? instance;

    private List<SocketGuildUser> newUsers;
    private List<IMessage> userMessages;

    private string WelcomeMessageStart => "Hello and welcome";

    private string WelcomeMessageEnd => "\nYou got access now. Follow rules and use brain please.";

    private int minMessageCount = 20;

    public NewUserHelper()
    {
        newUsers = new List<SocketGuildUser>();
        userMessages = new List<IMessage>();
    }

    public void StartAutomateUserWelcome()
    {
        automateUserWelcome = true;
    }

    public void StopAutomateUserWelcome()
    {
        automateUserWelcome = false;
    }

    public async Task AddWelcomeMessage(SocketMessage message, DiscordSocketClient client)
    {
        var guildUser = message.Author as SocketGuildUser;
        if(guildUser.HasRole("Possibly human"))
        {
            return;
        }
        
        if (automateUserWelcome && userMessages.Count == 0) {
            await Task.Delay(TimeSpan.FromMinutes(5));
            await AssignRoles(client);
        }

        userMessages.Add(message);
    }

    public Task AddUser(ulong id, SocketGuild guild)
    {
        var user = guild.GetUser(id);
        if (user.HasRole("Possibly human")) {
            return Task.CompletedTask;
        }
        
        newUsers.Add(user);
        return Task.CompletedTask;
    }

    public void RemoveUser(SocketGuildUser user, SocketGuild guild)
    {
        newUsers.Remove(user);
    }

    public bool HasUser(SocketUser user) => newUsers.Contains(user);

    public async Task AssignRoles(DiscordSocketClient context)
    {
        if (userMessages.Count == 0) {
            return;
        }

        var mentionedUsers = new List<string>();
        var shortIntroList = new List<string>();
        var welcomeChannel = context.GetChannel(HardcodedShit.welcomeId) as IMessageChannel;
        var guild = context.GetGuild(HardcodedShit.guildId);

        for (int i = 0; i < userMessages.Count; i++) {
            var m = userMessages[i];
            var user = guild.GetUser(m.Id);
            if (m.Author.Id != user.Id) {
                continue;
            }

            mentionedUsers.Add(user.Mention);
            newUsers.Remove(user);
            userMessages.Remove(m);

            if (m.Content.Length < minMessageCount) {
                shortIntroList.Add(user.Mention);
            }

            await user.AddRoleAsync(HardcodedShit.humanRoleId);
            break;
        }

        if (mentionedUsers.Count == 0) {
            return;
        }

        var phil = guild.GetUser(HardcodedShit.phil);
        var message = $"{WelcomeMessageStart} {string.Join(", ", mentionedUsers)}. {WelcomeMessageEnd} {phil.Mention}";

        await welcomeChannel.SendMessageAsync(message);

        if (shortIntroList.Count > 0) {
            var addendum =
                $"\n{string.Join(", ", shortIntroList)}, your intros seem a little short, maybe add a bit more? You have been granted access regardless!";
            await welcomeChannel.SendMessageAsync(addendum);
        }
    }
}