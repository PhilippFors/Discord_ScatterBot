using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Data;
using ScatterBot_v2.core.Extensions;
using ScatterBot_v2.core.Serialization;

namespace ScatterBot_v2.core.Helpers
{
    public class BonkedHelper
    {
        private DiscordClient context;
        private SaveSystem saveSystem;
        private List<BonkedMember> bonkedMembers => Moderation.bonkedMembers;

        public BonkedHelper()
        {
            Moderation.bonkedMembers = new List<BonkedMember>();
        }

        public void Initialize(DiscordClient client, SaveSystem saveSystem)
        {
            this.saveSystem = saveSystem;
            context = client;
            CheckMembers();
        }

        public bool IsBonked(ulong id)
        {
            var find = bonkedMembers.Find(x => x.id == id);
            return find != null;
        }

        public int BonkedAmount() => bonkedMembers.Count;

        public async Task AddBonkedMember(ulong id, double bonkTimeMinutes)
        {
            var guild = await context.GetGuildAsync(Guild.guildId);
            var user = await guild.GetMemberAsync(id);
            if (user == null) {
                await context.LogToChannel($"User with {id} could not be found");
                return;
            }

            await BonkInternal(user, guild, bonkTimeMinutes);
        }

        private async Task BonkInternal(DiscordMember user, DiscordGuild guild, double bonkTimeMinutes)
        {
            var runCheck = bonkedMembers.Count == 0;
            
            var id = user.Id;

            var exist = bonkedMembers.Find(x => x.id == id);

            if (exist != null) {
                exist.bonkDuration += MinuteToSecond(bonkTimeMinutes);
                exist.endTime = exist.startTime + exist.bonkDuration;
            }
            else {
                var allRoles = user.Roles.ToList();
                var allRoleIds = new List<ulong>();
                foreach (var role in allRoles) {
                    allRoleIds.Add(role.Id);
                    await user.RevokeRoleAsync(role);
                }

                await user.GrantRoleAsync(
                    guild.GetRole(Roles.mutedRoleId)
                );

                var time = MinuteToSecond(bonkTimeMinutes);
                var bonked = new BonkedMember() {
                    bonkDuration = time,
                    id = id,
                    startTime = DateTime.Now.TimeOfDay.TotalSeconds,
                    endTime = DateTime.Now.TimeOfDay.TotalSeconds + time,
                    initialRoles = allRoleIds
                };

                Moderation.bonkedMembers.Add(bonked);
            }

            if (runCheck) {
                CheckMembers();
            }

            await guild.LogToChannel($"Bonked user {user.Username} for {bonkTimeMinutes:0.0} Minutes");

            saveSystem.SaveData();
        }

        public async Task UnbonkMember(BonkedMember member)
        {
            RemoveBonkedMember(member.id);

            var user = await context.GetUserAsync(member.id) as DiscordMember;

            foreach (var id in member.initialRoles) {
                await user.GrantRoleAsync(
                    await context.GetRole(id)
                );
            }

            await user.RevokeRoleAsync(
                await context.GetRole(Roles.mutedRoleId)
            );

            await context.LogToChannel($"Unbonked user {user.Username}");
        }

        public async Task UnbonkMember(ulong id)
        {
            var bonkedMember = Moderation.bonkedMembers.Find(x => x.id == id);

            if (bonkedMember == null) {
                return;
            }

            RemoveBonkedMember(id);

            var guild = await context.GetGuildAsync(Guild.guildId);
            var user = await guild.GetMemberAsync(id);
            foreach (var roleId in bonkedMember.initialRoles) {
                user.GrantRoleAsync(
                    await context.GetRole(roleId)
                );
            }

            await user.RevokeRoleAsync(
                await context.GetRole(Roles.mutedRoleId)
            );

            await context.LogToChannel($"Unbonked user {user.Username}");
        }

        private void RemoveBonkedMember(ulong id)
        {
            var member = Moderation.bonkedMembers.Find(x => x.id == id);
            if (member != null) {
                Moderation.bonkedMembers.Remove(member);
            }

            saveSystem.SaveData();
        }

        private async Task CheckMembers()
        {
            while (Moderation.bonkedMembers.Count > 0) {
                for (int i = 0; i < bonkedMembers.Count; i++) {
                    var member = bonkedMembers[i];
                    if (DateTime.Now.TimeOfDay.TotalMilliseconds > member.endTime) {
                        await UnbonkMember(member);
                    }
                }

                await Task.Delay(250);
            }
        }

        private double MinuteToSecond(double time)
        {
            return time * 60;
        }
    }
}