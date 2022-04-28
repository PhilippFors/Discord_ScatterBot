﻿using System;
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

        private List<BonkedMember> tempBonked;
        public BonkedHelper(SaveSystem saveSystem, DiscordClient context)
        {
            this.saveSystem = saveSystem;
            this.context = context;
            if (saveSystem.ServerData.bonkedMembers == null || saveSystem.ServerData.bonkedMembers.Length == 0) {
                tempBonked = new List<BonkedMember>();
            }
            else {
                tempBonked = saveSystem.ServerData.bonkedMembers.ToList();
            }

            CheckMembers();
        }

        public bool IsBonked(ulong id)
        {
            var find = tempBonked.Find(x => x.id == id);
            return find != null;
        }

        public int BonkedAmount() => tempBonked.Count;

        public async Task AddBonkedMember(ulong id, double bonkTimeMinutes)
        {
            var guild = await context.GetGuildAsync(Guild.guildId);
            var user = await guild.GetMemberAsync(id);
            if (user == null) {
                await context.LogToChannel($"User with {id} could not be found", saveSystem.ServerData.botLogChannel);
                return;
            }

            await BonkInternal(user, guild, bonkTimeMinutes);
        }

        private async Task BonkInternal(DiscordMember user, DiscordGuild guild, double bonkTimeMinutes)
        {
            var runCheck = tempBonked.Count == 0;

            var id = user.Id;

            var exist = tempBonked.Find(x => x.id == id);

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
                    guild.GetRole(saveSystem.ServerData.mutedRoleId)
                );

                var time = MinuteToSecond(bonkTimeMinutes);
                var bonked = new BonkedMember() {
                    bonkDuration = time,
                    id = id,
                    startTime = DateTime.Now.TimeOfDay.TotalSeconds,
                    endTime = DateTime.Now.TimeOfDay.TotalSeconds + time,
                    initialRoles = allRoleIds
                };

                tempBonked.Add(bonked);
            }

            if (runCheck) {
                CheckMembers();
            }

            await guild.LogToChannel($"Bonked user {user.Username} for {bonkTimeMinutes:0.0} Minutes", saveSystem.ServerData.botLogChannel);

            saveSystem.ServerData.bonkedMembers = tempBonked.ToArray();
            await saveSystem.SaveData();
        }

        public async Task UnbonkMember(BonkedMember member)
        {
            RemoveBonkedMember(member.id);
            var guild = await context.GetGuildAsync(Guild.guildId);
            var user = await guild.GetMemberAsync(member.id);
            
            foreach (var id in member.initialRoles) {
                await user.GrantRoleAsync(
                    await context.GetRole(id)
                );
            }

            await user.RevokeRoleAsync(
                await context.GetRole(saveSystem.ServerData.mutedRoleId)
            );

            await context.LogToChannel($"Unbonked user {user.Username}", saveSystem.ServerData.botLogChannel);
        }

        public async Task UnbonkMember(ulong id)
        {
            var bonkedMember = tempBonked.Find(x => x.id == id);

            if (bonkedMember == null) {
                return;
            }

            RemoveBonkedMember(id);

            var guild = await context.GetGuildAsync(Guild.guildId);
            var user = await guild.GetMemberAsync(id);
            foreach (var roleId in bonkedMember.initialRoles) {
                await user.GrantRoleAsync(
                    await context.GetRole(roleId)
                );
            }

            await user.RevokeRoleAsync(
                await context.GetRole(saveSystem.ServerData.mutedRoleId)
            );

            await context.LogToChannel($"Unbonked user {user.Username}", saveSystem.ServerData.botLogChannel);
        }

        private void RemoveBonkedMember(ulong id)
        {
            var member = tempBonked.Find(x => x.id == id);
            if (member != null) {
                tempBonked.Remove(member);
            }

            saveSystem.ServerData.bonkedMembers = tempBonked.ToArray();
            saveSystem.SaveData();
        }

        private async Task CheckMembers()
        {
            while (tempBonked.Count > 0) {
                for (int i = 0; i < tempBonked.Count; i++) {
                    var member = tempBonked[i];
                    if (DateTime.Now.TimeOfDay.TotalSeconds > member.endTime) {
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