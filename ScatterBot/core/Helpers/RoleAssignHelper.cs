using Discord;
using Discord.Commands;
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

    private string WelcomeMessageStart => "Welcome to the server";

    private string WelcomeMessageEnd =>
        "\nYou now have a role to access the server. Stick to the rules and have fun! If you are interested in helping out with playtesting, ask ";

    public RoleAssignHelper()
    {
        newUsers = new List<SocketGuildUser>();
    }

    public async Task AddUser(ulong id, SocketGuild guild)
    {
        var user = guild.GetUser(id);
        newUsers.Add(user);
    }

    public async Task AssignRoles(IDiscordClient context)
    {
        var mentionedUsers = new List<string>();
        for (int i = 0; i < newUsers.Count; i++) {
            var user = newUsers[i];
            mentionedUsers.Add(user.Mention);
            await user.AddRoleAsync(HardcodedShit.humanRoleId);
        }

        var message = WelcomeMessageStart;

        foreach (var m in mentionedUsers) {
            message += ", " + m;
        }

        message += "." + WelcomeMessageEnd;
        var guild = await context.GetGuildAsync(HardcodedShit.guildId);
        var phil = await guild.GetUserAsync(HardcodedShit.phil);
        message += phil.Mention;

        newUsers.Clear();
        await context.GetChannel("welcome").Result.SendMessageAsync(message);
    }
}