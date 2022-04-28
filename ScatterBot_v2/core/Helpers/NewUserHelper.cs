﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Data;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Serialization;

namespace ScatterBot_v2.core.Helpers;

public class NewUserHelper
{
    private string WelcomeMessageStart => "Hello and welcome";

    private string WelcomeMessageEnd => "\nYou got access now. Follow rules and use brain please.";

    private int minMessageCount = 20;
    private bool automateUserWelcome;

    private SaveSystem saveSystem;
    private ulong AccessRole => saveSystem.ServerData.accessRoleId;
    private List<ulong> newUsers;
    private List<MessageSaveData> introductions;

    public NewUserHelper(SaveSystem saveSystem)
    {
        this.saveSystem = saveSystem;
        Initialize();
    }

    private void Initialize()
    {
        newUsers =
            saveSystem.ServerData.newUsers == null ? new List<ulong>() : saveSystem.ServerData.newUsers.ToList();
        introductions =
            saveSystem.ServerData.newIntroductions == null
                ? new List<MessageSaveData>()
                : saveSystem.ServerData.newIntroductions.ToList();
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
        if (user.HasRole(AccessRole)) {
            return;
        }

        if (automateUserWelcome && introductions.Count == 0) {
            await Task.Delay(TimeSpan.FromMinutes(5));
            await AssignRoles(client);
        }

        introductions.Add(new MessageSaveData() {
            channel = message.ChannelId,
            messageId = message.Id,
            userId = user.Id
        });
        
        Save();
    }

    public Task AddUser(ulong id)
    {
        newUsers.Add(id);
        Save();
        return Task.CompletedTask;
    }

    public void RemoveUser(ulong user)
    {
        if (newUsers.Contains(user)) {
            newUsers.Remove(user);
            Save();
        }
    }

    public bool HasUser(DiscordMember user) => newUsers.Contains(user.Id);

    public async Task AssignRoles(DiscordClient context)
    {
        if (introductions.Count == 0) {
            return;
        }

        var mentionedUsers = new List<string>();
        var shortIntroList = new List<string>();

        var guild = await context.GetGuildAsync(Guild.guildId);
        var welcomeChannel = guild.GetChannel(AccessRole);

        for (int i = 0; i < introductions.Count; i++) {
            var data = introductions[i];
            var userMessage = await welcomeChannel.GetMessageAsync(data.messageId);
            var user = await guild.GetMemberAsync(data.userId);

            if (user.HasRole(saveSystem.ServerData.accessRoleId)) {
                continue;
            }

            mentionedUsers.Add(user.Mention);
            newUsers.Remove(user.Id);
            introductions.Remove(data);

            if (userMessage.Content.Length < minMessageCount) {
                shortIntroList.Add(user.Mention);
            }

            await user.GrantRoleAsync(
                guild.GetRole(AccessRole)
            );
            break;
        }

        if (mentionedUsers.Count == 0) {
            return;
        }

        var phil = await guild.GetMemberAsync(Moderation.philUserId);

        var message = $"{WelcomeMessageStart} {string.Join(", ", mentionedUsers)}. {WelcomeMessageEnd} {phil.Mention}";

        await welcomeChannel.SendMessageAsync(message);

        if (shortIntroList.Count > 0) {
            var addendum =
                $"\n{string.Join(", ", shortIntroList)}, your intros seem a little short, maybe add a bit more? You have been granted access regardless!";
            await welcomeChannel.SendMessageAsync(addendum);
        }

        Save();
    }

    private void Save()
    {
        saveSystem.ServerData.newIntroductions = introductions.ToArray();
        saveSystem.ServerData.newUsers = newUsers.ToArray();
        saveSystem.SaveData();
    }
}