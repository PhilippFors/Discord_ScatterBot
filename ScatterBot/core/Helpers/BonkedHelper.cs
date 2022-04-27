using Discord;
using ScatterBot.core.Extensions;

namespace ScatterBot.core.Helpers
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

        public async Task AddBonkedMember(ulong id, double bonkTimeMinutes, IDiscordClient context)
        {
            var guild = await context.GetGuildAsync(HardcodedShit.guildId);
            var guildUser = await guild.GetUserAsync(id);

            if (guildUser == null) {
                await context.LogToChannel($"User with {id} could not be found");
                return;
            }

            await BonkInternal(guildUser, guild.Id, bonkTimeMinutes, context);
        }

        private async Task BonkInternal(IGuildUser guildUser, ulong guildId, double bonkTimeMinutes,
            IDiscordClient client)
        {
            bool runCheck = bonkedMembers.Count == 0;

            var id = guildUser.Id;

            var exist = bonkedMembers.Find(x => x.id == id);

            if (exist != null) {
                exist.BonkDuration += MinuteToMilliseconds(bonkTimeMinutes);
            }
            else {
                var allRoles = guildUser.RoleIds.ToList();
                allRoles.RemoveAt(0);

                foreach (var role in allRoles) {
                    await guildUser.RemoveRoleAsync(role);
                }

                await guildUser.AddRoleAsync(HardcodedShit.bonkedRoleId);
                var bonked = new BonkedMember(id, guildId, MinuteToMilliseconds(bonkTimeMinutes), allRoles);
                bonkedMembers.Add(bonked);
            }

            if (runCheck) {
                CheckMembers(client);
            }

            await client.LogToChannel(
                $"Bonked user {guildUser.Username} for {bonkTimeMinutes.ToString("0.0")} Minutes");
        }

        public async Task UnbonkMember(BonkedMember id, IDiscordClient context)
        {
            RemoveBonkedMember(id.id);

            var guild = await context.GetGuildAsync(id.guildId);
            var guildUser = await guild.GetUserAsync(id.id);
            await guildUser.AddRolesAsync(id.initialRoles);
            await guildUser.RemoveRoleAsync(HardcodedShit.bonkedRoleId);

            await context.LogToChannel($"Unbonked user {guildUser.Username}");
        }

        public async Task UnbonkMember(ulong id, IDiscordClient context)
        {
            var bonkedMember = bonkedMembers.Find(x => x.id == id);

            if (bonkedMember == null) {
                return;
            }

            RemoveBonkedMember(id);

            var guild = await context.GetGuildAsync(bonkedMember.guildId);
            var guildUser = await guild.GetUserAsync(bonkedMember.id);
            await guildUser.AddRolesAsync(bonkedMember.initialRoles);
            await guildUser.RemoveRoleAsync(HardcodedShit.bonkedRoleId);

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
        public BonkedMember(ulong id, ulong guildId, double duration, List<ulong> initialRoles)
        {
            this.id = id;
            this.guildId = guildId;
            bonkDuration = duration;
            startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
            endTime = startTime + duration;
            this.initialRoles = initialRoles;
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