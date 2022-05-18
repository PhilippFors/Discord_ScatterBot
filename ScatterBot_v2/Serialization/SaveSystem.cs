using System.Collections.Generic;
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
        // public ServerData ServerData => serverData;
        // private ServerData serverData;
        private string appPath => Directory.GetCurrentDirectory();
        private string configPath = "/config/";
        private string SavePath => appPath + configPath;
        private Dictionary<string, object> dataCache = new Dictionary<string, object>();

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
            using var file = File.OpenWrite(SavePath + typeof(T));
            Serializer.Serialize(file, data);
        }

        public T LoadAs<T>() where T : new()
        {
            if(dataCache.ContainsKey(typeof(T).ToString()))
            {
                return (T)dataCache[typeof(T).ToString()];
            }
            
            var path = SavePath + typeof(T);
            T data = default;
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                data = new T();
                dataCache.Add(typeof(T).ToString(), data);
                return data;
            }

            using var file = File.OpenRead(path);
            data = Serializer.Deserialize<T>(file);
            dataCache.Add(typeof(T).ToString(), data);
            return data;
        }
    }
}