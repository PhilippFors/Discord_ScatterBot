using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.Data;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Services
{
    /// <summary>
    /// Tracks new users and introduction messages.
    /// Can assign access roles in bulk to new users with welcome messages and notifies the new users.
    /// </summary>
    public class NewUserHelperService : ISaveable
    {
        private string WelcomeMessageStart => "Hello and welcome";

        private string WelcomeMessageEnd => "\nYou got access now. Follow rules and use brain please.";

        private int minMessageCount = 20;
        private bool automateUserWelcome;

        private readonly SaveSystem saveSystem;
        private ServerData serverData;
        private ChannelConfigs channelConfigs;
        private RoleData roleData;
        private List<ulong> newUsers;
        private List<MessageSaveData> introductions;

        public NewUserHelperService(SaveSystem saveSystem)
        {
            this.saveSystem = saveSystem;
            Load();
        }

        public void StartAutomateUserWelcome()
        {
            automateUserWelcome = true;
        }

        public void StopAutomateUserWelcome()
        {
            automateUserWelcome = false;
        }

        public async Task AddWelcomeMessage(DiscordMessage message)
        {
            var guild = Guild.guild;
            var user = await guild.GetMemberAsync(message.Author.Id);
            if (user.HasRole(roleData.accessRoleId)) {
                return;
            }

            if (automateUserWelcome && introductions.Count == 0) {
                await Task.Delay(TimeSpan.FromMinutes(5));
                await AssignRoles();
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

        public async Task AssignRoles()
        {
            if (introductions.Count == 0) {
                return;
            }

            var mentionedUsers = new List<string>();
            var shortIntroList = new List<string>();

            var guild = Guild.guild;
            var welcomeChannel = guild.GetChannel(channelConfigs.welcomeChannel);

            for (int i = 0; i < introductions.Count; i++) {
                var data = introductions[i];
                var user = await guild.GetMemberAsync(data.userId);

                if (user.HasRole(roleData.accessRoleId)) {
                    continue;
                }
                
                var userMessage = await welcomeChannel.GetMessageAsync(data.messageId);
                mentionedUsers.Add(user.Mention);
                newUsers.Remove(user.Id);
                introductions.Remove(data);

                if (userMessage.Content.Length < minMessageCount) {
                    shortIntroList.Add(user.Mention);
                }

                await user.GrantRoleAsync(
                    guild.GetRole(roleData.accessRoleId)
                );
                break;
            }

            if (mentionedUsers.Count == 0) {
                return;
            }

            var message = $"{WelcomeMessageStart} {string.Join(", ", mentionedUsers)}. {WelcomeMessageEnd}";

            await welcomeChannel.SendMessageAsync(message);

            if (shortIntroList.Count > 0) {
                var s = shortIntroList.Count > 1 ? "s" : "";
                var addendum =
                    $"\n{string.Join(", ", shortIntroList)}, your intro{s} seem a little short, maybe add a bit more? You have been granted access regardless!";
                await welcomeChannel.SendMessageAsync(addendum);
            }

            Save();
        }
        
        public void Save()
        {
            serverData.newIntroductions = introductions.ToArray();
            serverData.newUsers = newUsers.ToArray();
            saveSystem.SaveAs<ServerData>(serverData);
        }
        
        public void Load()
        {
            channelConfigs = saveSystem.LoadAs<ChannelConfigs>();
            roleData = saveSystem.LoadAs<RoleData>();
            serverData = saveSystem.LoadAs<ServerData>();
            newUsers = serverData.newUsers == null ? new List<ulong>() : serverData.newUsers.ToList();
            introductions = serverData.newIntroductions == null ? new List<MessageSaveData>() : serverData.newIntroductions.ToList();
        }
    }
}