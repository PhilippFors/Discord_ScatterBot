using Discord.Commands;
using Discord.WebSocket;

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
            
            var exist = bonkedMembers.Find(x => x.id == id);
            
            bool runCheck = bonkedMembers.Count == 0;

            if (exist != null) {
                exist.BonkDuration += MinuteToMilliseconds(bonkTimeMinutes);
            }
            else {
                var allRoles = guildUser.Roles.ToList();
                allRoles.RemoveAt(0);

                foreach (var role in allRoles) {
                    await guildUser.RemoveRoleAsync(role.Id);
                }

                await guildUser.AddRoleAsync(bonkedRole);
                var bonked = new BonkedMember(id, MinuteToMilliseconds(bonkTimeMinutes), allRoles);
                bonkedMembers.Add(bonked);
            }

            if (runCheck) {
                CheckMembers(context);
            }

            await context.LogToChannel($"Bonked user {guildUser.Username} for {bonkTimeMinutes.ToString("0.0")} Minutes");
        }

        public async Task UnbonkMember(ulong id, SocketCommandContext context)
        {
            var guild = context.Guild;
            var guildUser = guild.GetUser(id);
            var bonkedMember = bonkedMembers.Find(x => x.id == id);
            if (bonkedMember == null) {
                return;
            }
            await guildUser.AddRolesAsync(bonkedMember.initialRoles);
            await guildUser.RemoveRoleAsync(bonkedRole);

            RemoveBonkedMember(id);

            await context.LogToChannel($"Unbonked user {guildUser.Username}");
        }

        public async Task UnbonkMember(BonkedMember id, SocketCommandContext context)
        {
            var guild = context.Guild;
            var guildUser = guild.GetUser(id.id);
            await guildUser.AddRolesAsync(id.initialRoles);
            await guildUser.RemoveRoleAsync(bonkedRole);

            RemoveBonkedMember(id.id);
        
            await context.LogToChannel($"Unbonked user {guildUser.Username}");
        }

        public void RemoveBonkedMember(ulong id)
        {
            var member = bonkedMembers.Find(x => x.id == id);
            if (member != null) {
                bonkedMembers.Remove(member);
            }
        }

        public async Task CheckMembers(SocketCommandContext context)
        {
            while (bonkedMembers.Count > 0) {
                for (int i = 0; i < bonkedMembers.Count; i++) {
                    var member = bonkedMembers[i];
                    if (DateTime.Now.TimeOfDay.TotalMilliseconds > member.endTime.TimeOfDay.TotalMilliseconds) {
                        await UnbonkMember(member, context);
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
        public BonkedMember(ulong id, double duration, List<SocketRole> initialRoles)
        {
            this.id = id;
            bonkDuration = duration;
            startTime = DateTime.Now;
            endTime = DateTime.Now.AddMilliseconds(BonkDuration);
            this.initialRoles = initialRoles;
        }

        public List<SocketRole> initialRoles;
        public ulong id;
        public DateTime startTime;
        public DateTime endTime;

        public double BonkDuration {
            get {
                return bonkDuration;
            }
            set {
                bonkDuration = value;
                endTime = startTime.AddMilliseconds(bonkDuration);
            }
        }

        private double bonkDuration; // in milliseconds
    }
}