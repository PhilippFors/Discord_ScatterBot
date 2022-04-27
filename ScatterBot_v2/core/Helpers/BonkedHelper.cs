using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Extensions;

namespace ScatterBot_v2.core.Helpers
{
    public class BonkedHelper
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

        private readonly List<BonkedMember> bonkedMembers;

        public BonkedHelper()
        {
            bonkedMembers = new List<BonkedMember>();
        }

        public bool IsBonked(ulong id)
        {
            var find = bonkedMembers.Find(x => x.id == id);
            return find != null;
        }

        public async Task AddBonkedMember(ulong id, double bonkTimeMinutes, DiscordClient context)
        {
            var guild = await context.GetGuildAsync(HardcodedShit.guildId);
            var user = await guild.GetMemberAsync(id);
            if (user == null) {
                await context.LogToChannel($"User with {id} could not be found");
                return;
            }

            await BonkInternal(user, guild, bonkTimeMinutes, context);
        }

        private async Task BonkInternal(DiscordMember user, DiscordGuild guild, double bonkTimeMinutes,
            DiscordClient context)
        {
            bool runCheck = bonkedMembers.Count == 0;

            var id = user.Id;

            var exist = bonkedMembers.Find(x => x.id == id);

            if (exist != null) {
                exist.BonkDuration += MinuteToMilliseconds(bonkTimeMinutes);
            }
            else {
                var allRoles = user.Roles.ToList();
                allRoles.RemoveAt(0);

                foreach (var role in allRoles) {
                    await user.RevokeRoleAsync(role);
                }

                await user.GrantRoleAsync(
                    await context.GetRole(HardcodedShit.bonkedRoleId)
                );

                var bonked = new BonkedMember(id, guild.Id, MinuteToMilliseconds(bonkTimeMinutes), allRoles);
                bonkedMembers.Add(bonked);
            }

            if (runCheck) {
                CheckMembers(context);
            }

            await context.LogToChannel($"Bonked user {user.Username} for {bonkTimeMinutes:0.0} Minutes");
        }

        public async Task UnbonkMember(BonkedMember member, DiscordClient context)
        {
            RemoveBonkedMember(member.id);

            var user = await context.GetUserAsync(member.id) as DiscordMember;

            foreach (var id in member.initialRoles) {
                await user.GrantRoleAsync(
                    await context.GetRole(id)
                );
            }

            await user.RevokeRoleAsync(
                await context.GetRole(HardcodedShit.bonkedRoleId)
            );

            await context.LogToChannel($"Unbonked user {user.Username}");
        }

        public async Task UnbonkMember(ulong id, DiscordClient context)
        {
            var bonkedMember = bonkedMembers.Find(x => x.id == id);

            if (bonkedMember == null) {
                return;
            }

            RemoveBonkedMember(id);

            var guild = await context.GetGuildAsync(bonkedMember.guildId);
            var user = await guild.GetMemberAsync(id);
            foreach (var roleId in bonkedMember.initialRoles) {
                user.GrantRoleAsync(
                    await context.GetRole(roleId)
                );
            }

            await user.RevokeRoleAsync(
                await context.GetRole(HardcodedShit.bonkedRoleId)
            );

            await context.LogToChannel($"Unbonked user {user.Username}");
        }

        private void RemoveBonkedMember(ulong id)
        {
            var member = bonkedMembers.Find(x => x.id == id);
            if (member != null) {
                bonkedMembers.Remove(member);
            }
        }

        public async Task CheckMembers(DiscordClient client)
        {
            while (bonkedMembers.Count > 0) {
                for (int i = 0; i < bonkedMembers.Count; i++) {
                    var member = bonkedMembers[i];
                    if (DateTime.Now.TimeOfDay.TotalMilliseconds > member.endTime) {
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
        public BonkedMember(ulong id, ulong guildId, double duration, List<DiscordRole> initialRoles)
        {
            this.id = id;
            this.guildId = guildId;
            bonkDuration = duration;
            startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
            endTime = startTime + duration;
            this.initialRoles = new List<ulong>();
            initialRoles.ForEach(x => this.initialRoles.Add(x.Id));
        }

        public List<ulong> initialRoles;
        public ulong id;
        public ulong guildId;
        public double startTime;
        public double endTime;

        public double BonkDuration {
            get { return bonkDuration; }
            set {
                bonkDuration += value;
                endTime = startTime + bonkDuration;
            }
        }

        private double bonkDuration; // in milliseconds
    }
}