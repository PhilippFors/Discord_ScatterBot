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
    /// Helps with assigning a muted role to a user and checking if the specified muted time has expired.
    /// </summary>
    public class MuteHelperService
    {
        private SaveSystem saveSystem;

        private List<BonkedMember> tempBonked;

        public MuteHelperService(SaveSystem saveSystem)
        {
            this.saveSystem = saveSystem;
            if (saveSystem.ServerData.bonkedMembers == null || saveSystem.ServerData.bonkedMembers.Length == 0)
            {
                tempBonked = new List<BonkedMember>();
            }
            else
            {
                tempBonked = saveSystem.ServerData.bonkedMembers.ToList();
            }

            CheckMembers();
        }

        public bool IsBonked(ulong id)
        {
            return tempBonked.Exists(x => x.id == id);
        }

        public int BonkedAmount() => tempBonked.Count;

        public async Task AddBonkedMember(ulong id, double bonkTimeMinutes)
        {
            var guild = Guild.guild;
            var user = await guild.GetMemberAsync(id);
            if (user == null)
            {
                await guild.LogToChannel($"User with {id} could not be found", saveSystem.ServerData.botLogChannel);
                return;
            }

            await BonkInternal(user, guild, bonkTimeMinutes);
        }

        private async Task BonkInternal(DiscordMember user, DiscordGuild guild, double bonkTimeMinutes)
        {
            var runCheck = tempBonked.Count == 0;

            var id = user.Id;

            var exist = tempBonked.Find(x => x.id == id);

            if (exist != null)
            {
                exist.bonkDuration += MinuteToSecond(bonkTimeMinutes);
                exist.endTime = exist.startTime + exist.bonkDuration;
            }
            else
            {
                var allRoles = user.Roles.ToList();
                var allRoleIds = new List<ulong>();
                foreach (var role in allRoles)
                {
                    allRoleIds.Add(role.Id);
                    await user.RevokeRoleAsync(role);
                }

                await user.GrantRoleAsync(
                    guild.GetRole(saveSystem.ServerData.mutedRoleId)
                );

                var time = MinuteToSecond(bonkTimeMinutes);
                var bonked = new BonkedMember()
                {
                    bonkDuration = time,
                    id = id,
                    startTime = DateTime.Now.TimeOfDay.TotalSeconds,
                    endTime = DateTime.Now.TimeOfDay.TotalSeconds + time,
                    initialRoles = allRoleIds
                };

                tempBonked.Add(bonked);
            }

            if (runCheck)
            {
                CheckMembers();
            }

            await guild.LogToChannel($"Bonked user {user.Username} for {bonkTimeMinutes:0.0} Minutes",
                saveSystem.ServerData.botLogChannel);

            saveSystem.ServerData.bonkedMembers = tempBonked.ToArray();
            await saveSystem.SaveData();
        }

        public async Task UnbonkMember(BonkedMember member)
        {
            RemoveBonkedMember(member.id);
            var guild = Guild.guild;
            var user = await guild.GetMemberAsync(member.id);

            foreach (var id in member.initialRoles)
            {
                await user.GrantRoleAsync(
                    guild.GetRole(id)
                );
            }

            await user.RevokeRoleAsync(
                guild.GetRole(saveSystem.ServerData.mutedRoleId)
            );

            await guild.LogToChannel($"Unbonked user {user.Username}", saveSystem.ServerData.botLogChannel);
        }

        public async Task UnbonkMember(ulong id)
        {
            var bonkedMember = tempBonked.Find(x => x.id == id);

            if (bonkedMember == null)
            {
                return;
            }

            RemoveBonkedMember(id);

            var guild = Guild.guild;
            var user = await guild.GetMemberAsync(id);
            foreach (var roleId in bonkedMember.initialRoles)
            {
                await user.GrantRoleAsync(
                    guild.GetRole(roleId)
                );
            }

            await user.RevokeRoleAsync(
                guild.GetRole(saveSystem.ServerData.mutedRoleId)
            );

            await guild.LogToChannel($"Unbonked user {user.Username}", saveSystem.ServerData.botLogChannel);
        }

        private void RemoveBonkedMember(ulong id)
        {
            var member = tempBonked.Find(x => x.id == id);
            if (member != null)
            {
                tempBonked.Remove(member);
            }

            saveSystem.ServerData.bonkedMembers = tempBonked.ToArray();
            saveSystem.SaveData();
        }

        private async Task CheckMembers()
        {
            while (tempBonked.Count > 0)
            {
                for (int i = 0; i < tempBonked.Count; i++)
                {
                    var member = tempBonked[i];
                    if (member.bonkDuration < 0)
                    {
                        continue;
                    }

                    if (DateTime.Now.TimeOfDay.TotalSeconds > member.endTime)
                    {
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