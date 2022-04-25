﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ScatterBot.core.Helpers
{
    public partial class BonkedHelper
    {
        public static BonkedHelper Instance {
            get {
                if (instance == null) {
                    instance = new BonkedHelper();
                }

                return instance;
            }
        }

        private static BonkedHelper? instance;

        private List<BonkedMember> bonkedMembers;
        private ulong bonkedRole => 967594914630217798;

        public BonkedHelper()
        {
            bonkedMembers = new List<BonkedMember>();
        }
        
        public async Task AddBonkedMember(ulong id, double bonkTimeMinutes, SocketCommandContext context)
        {
            var guild = context.Guild;
            var guildUser = guild.GetUser(id);

            if (guildUser == null) {
                await context.LogToChannel($"User with {id} could not be found");
                return;
            }

            await BonkInternal(guildUser, guild.Id, bonkTimeMinutes, context.Client);
        }
        
        private async Task BonkInternal(SocketGuildUser guildUser, ulong guildId, double bonkTimeMinutes, IDiscordClient client)
        {
            bool runCheck = bonkedMembers.Count == 0;

            var id = guildUser.Id;
        
            var exist = bonkedMembers.Find(x => x.id == id);

            if (exist != null) {
                exist.BonkDuration += MinuteToMilliseconds(bonkTimeMinutes);
            }
            else {
                var allRoles = guildUser.Roles.ToList();
                allRoles.RemoveAt(0);

                foreach (var role in allRoles) {
                    await guildUser.RemoveRoleAsync(role);
                }

                await guildUser.AddRoleAsync(bonkedRole);
                var bonked = new BonkedMember(id, guildId, MinuteToMilliseconds(bonkTimeMinutes), allRoles);
                bonkedMembers.Add(bonked);
            }

            if (runCheck) {
                CheckMembers(client);
            }

            await client.LogToChannel($"Bonked user {guildUser.Username} for {bonkTimeMinutes.ToString("0.0")} Minutes");
        }
        
        public async Task UnbonkMember(BonkedMember id, IDiscordClient context)
        {
            var guild = await context.GetGuildAsync(id.guildId);
            var guildUser = await guild.GetUserAsync(id.id);
            await guildUser.AddRolesAsync(id.initialRoles);
            await guildUser.RemoveRoleAsync(bonkedRole);

            RemoveBonkedMember(id.id);

            await context.LogToChannel($"Unbonked user {guildUser.Username}");
        }

        private void RemoveBonkedMember(ulong id)
        {
            var member = bonkedMembers.Find(x => x.id == id);
            if (member != null) {
                bonkedMembers.Remove(member);
            }
        }

        public async Task CheckMembers(IDiscordClient client)
        {
            while (bonkedMembers.Count > 0) {
                for (int i = 0; i < bonkedMembers.Count; i++) {
                    var member = bonkedMembers[i];
                    if (DateTime.Now.TimeOfDay.TotalMilliseconds > member.endTime.TimeOfDay.TotalMilliseconds) {
                        await UnbonkMember(member, client);
                    }
                }

                await Task.Delay(250);
            }
        }
        
        private double MinuteToMilliseconds(double time)
        {
            return time * 60 * 1000;
        }
    }

    public class BonkedMember
    {
        public BonkedMember(ulong id, ulong guildId, double duration, List<SocketRole> initialRoles)
        {
            this.id = id;
            this.guildId = guildId;
            bonkDuration = duration;
            startTime = DateTime.Now;
            endTime = DateTime.Now.AddMilliseconds(BonkDuration);
            this.initialRoles = initialRoles;
        }

        public List<SocketRole> initialRoles;
        public ulong id;
        public ulong guildId;
        public DateTime startTime;
        public DateTime endTime;

        public double BonkDuration {
            get { return bonkDuration; }
            set {
                bonkDuration = value;
                endTime = startTime.AddMilliseconds(bonkDuration);
            }
        }

        private double bonkDuration; // in milliseconds
    }
}