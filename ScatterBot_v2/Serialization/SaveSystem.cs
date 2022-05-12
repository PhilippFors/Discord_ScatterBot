using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using ProtoBuf;
using ScatterBot_v2.Data;

namespace ScatterBot_v2.Serialization
{
    /// <summary>
    /// Serializes and deserializes <see cref="ServerData"/>.
    /// Initialized runtime data.
    /// </summary>
    public class SaveSystem
    {
        public ServerData ServerData => serverData;
        private ServerData serverData;
        private string appPath => Directory.GetCurrentDirectory();
        private string configPath = "/config/";
        private string configName = "saveData.save";
        private string FullPath => appPath + configPath + configName;

        public void LoadData()
        {
            if (!Directory.Exists(appPath + configPath))
            {
                Directory.CreateDirectory(appPath + configPath);
            }

            if (!File.Exists(FullPath))
            {
                File.Create(FullPath).Close();
                serverData = new ServerData();
                return;
            }

            using var file = File.OpenRead(FullPath);

            serverData = Serializer.Deserialize<ServerData>(file);
        }

        public Task SaveData()
        {
            using var file = File.OpenWrite(FullPath);

            Serializer.Serialize(file, serverData);
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
            foreach (var ch in channels)
            {
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
            foreach (var rolePair in allRoles)
            {
                Roles.allRoleIds[index] = rolePair.Key;
                Roles.allRoleNames[index] = rolePair.Value.Name;
                index++;
            }

            return Task.CompletedTask;
        }

        public void SaveAs<T>(T data)
        {
            using var file = File.OpenWrite(appPath + configPath + nameof(T));
            Serializer.Serialize(file, data);
        }

        public T LoadAs<T>() where T : new()
        {
            var path = appPath + configPath + nameof(T);
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                var t = new T();
                return t;
            }

            using var file = File.OpenRead(path);
            return Serializer.Deserialize<T>(file);
        }
    }
}