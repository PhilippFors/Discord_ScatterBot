using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using ProtoBuf;
using ScatterBot_v2.Data;

namespace ScatterBot_v2.Serialization
{
    public class SaveSystem
    {
        public ServerData ServerData => serverData;
        private ServerData serverData;
        private string appPath => Directory.GetCurrentDirectory();
        private string configPath = "/config/";
        private string configName = "saveData.save";
        private string FullPath => appPath + configPath + configName;

        public SaveSystem()
        {
            InitProcessId();
        }
        
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

            serverData = Serializer.Deserialize<ServerData>(file);
        }

        private void InitProcessId()
        {
            File.CreateText(appPath + "/processId.txt").Close();
            using var writer = File.AppendText(appPath + "/processId.txt");
            writer.Write(Process.GetCurrentProcess().Id.ToString());
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
}