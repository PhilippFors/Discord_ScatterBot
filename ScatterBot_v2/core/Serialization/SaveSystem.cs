using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using ProtoBuf;
using ScatterBot_v2.core.Data;

namespace ScatterBot_v2.core.Serialization;

public class SaveSystem
{
    private ServerData serverData;
    private string appPath => Directory.GetCurrentDirectory();
    private string configPath = "/config/";
    private string configName = "saveData.save";
    private string FullPath => appPath + configPath + configName;

    public void LoadData()
    {
        if (!Directory.Exists(appPath + configPath)) {
            Directory.CreateDirectory(appPath + configPath);
        }

        if (!File.Exists(FullPath)) {
            File.Create(FullPath).Close();
            serverData = new ServerData();
            return;
        }

        using var file = File.OpenRead(FullPath);
        {
            serverData = Serializer.Deserialize<ServerData>(file);
        }

        Roles.accessRoleId = serverData.accessRoleId;
        Roles.mutedRoleId = serverData.mutedRoleId;
        Channels.welcomeChannelId = serverData.welcomeChannel;
        Channels.logChannelId = serverData.botLogChannel;
        Moderation.bonkedMembers = serverData.bonkedMembers;
        Moderation.newIntroductions = serverData.newIntroductions;
        Moderation.newUsers = serverData.newUsers;
    }

    public Task SaveData()
    {
        serverData.accessRoleId = Roles.accessRoleId;
        serverData.mutedRoleId = Roles.mutedRoleId;
        serverData.welcomeChannel = Channels.welcomeChannelId;
        serverData.botLogChannel = Channels.logChannelId;
        serverData.bonkedMembers = Moderation.bonkedMembers;
        serverData.newIntroductions = Moderation.newIntroductions;
        serverData.newUsers = Moderation.newUsers;

        using var file = File.Open(FullPath, FileMode.Open);
        
        Serializer.Serialize(file, serverData);
        Console.WriteLine("Saved!");
        return Task.CompletedTask;
    }

    public async Task InitializeRuntimeData(DiscordGuild guild)
    {
        await InitChannels(guild);
        await InitRoles(guild);
    }

    private async Task InitChannels(DiscordGuild guild)
    {
        var channels = await guild.GetChannelsAsync();

        Channels.allChannelIds = new ulong[channels.Count];
        Channels.allChannelNames = new string[channels.Count];

        int index = 0;
        foreach (var ch in channels) {
            Channels.allChannelIds[index] = ch.Id;
            Channels.allChannelNames[index] = ch.Name;
            index++;
        }
    }

    private Task InitRoles(DiscordGuild guild)
    {
        var allRoles = guild.Roles;

        Roles.allRoleIds = new ulong[allRoles.Count];
        Roles.allRoleNames = new string[allRoles.Count];

        int index = 0;
        foreach (var rolePair in allRoles) {
            Roles.allRoleIds[index] = rolePair.Key;
            Roles.allRoleNames[index] = rolePair.Value.Name;
            index++;
        }

        return Task.CompletedTask;
    }
}