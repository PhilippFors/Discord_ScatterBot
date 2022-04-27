using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;

namespace ScatterBot_v2.core.Helpers;

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

    private static NewUserHelper? instance;

    private readonly List<DiscordMember> newUsers;
    private readonly List<DiscordMessage> userMessages;

    private string WelcomeMessageStart => "Hello and welcome";

    private string WelcomeMessageEnd => "\nYou got access now. Follow rules and use brain please.";

    private int minMessageCount = 20;
    private bool automateUserWelcome;

    public NewUserHelper()
    {
        newUsers = new List<DiscordMember>();
        userMessages = new List<DiscordMessage>();
    }

    public void StartAutomateUserWelcome()
    {
        automateUserWelcome = true;
    }

    public void StopAutomateUserWelcome()
    {
        automateUserWelcome = false;
    }

    public async Task AddWelcomeMessage(DiscordMessage message, DiscordClient client)
    {
        var user = message.Author as DiscordMember;
        if (user.HasRole(HardcodedShit.humanRoleId)) {
            return;
        }

        if (automateUserWelcome && userMessages.Count == 0) {
            await Task.Delay(TimeSpan.FromMinutes(5));
            await AssignRoles(client);
        }

        userMessages.Add(message);
    }

    public async Task AddUser(ulong id, DiscordGuild guild)
    {
        var user = await guild.GetMemberAsync(id);
        if (user.HasRole(HardcodedShit.humanRoleId)) {
            return;
        }

        newUsers.Add(user);
    }

    public void RemoveUser(DiscordMember user, DiscordGuild guild)
    {
        newUsers.Remove(user);
    }

    public bool HasUser(DiscordMember user) => newUsers.Contains(user);

    public async Task AssignRoles(DiscordClient context)
    {
        if (userMessages.Count == 0) {
            return;
        }

        var mentionedUsers = new List<string>();
        var shortIntroList = new List<string>();

        var guild = await context.GetGuildAsync(HardcodedShit.guildId);
        var welcomeChannel = guild.GetChannel(HardcodedShit.welcomeId);

        for (int i = 0; i < userMessages.Count; i++) {
            var m = userMessages[i];
            var user = await guild.GetMemberAsync(m.Id);
            if (m.Author.Id != user.Id) {
                continue;
            }

            mentionedUsers.Add(user.Mention);
            newUsers.Remove(user);
            userMessages.Remove(m);

            if (m.Content.Length < minMessageCount) {
                shortIntroList.Add(user.Mention);
            }

            await user.GrantRoleAsync(
                guild.GetRole(HardcodedShit.humanRoleId)
            );
            break;
        }

        if (mentionedUsers.Count == 0) {
            return;
        }

        var phil = await guild.GetMemberAsync(HardcodedShit.phil);
        var message = $"{WelcomeMessageStart} {string.Join(", ", mentionedUsers)}. {WelcomeMessageEnd} {phil.Mention}";

        await welcomeChannel.SendMessageAsync(message);

        if (shortIntroList.Count > 0) {
            var addendum = $"\n{string.Join(", ", shortIntroList)}, your intros seem a little short, maybe add a bit more? You have been granted access regardless!";
            await welcomeChannel.SendMessageAsync(addendum);
        }
    }
}