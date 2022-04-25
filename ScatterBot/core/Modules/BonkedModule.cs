using Discord.Commands;
using Discord.WebSocket;

namespace ScatterBot.core.Modules;

public class BonkedModule
{
    public static BonkedModule Instance {
        get {
            if (instance == null) {
                instance = new BonkedModule();
            }

            return instance;
        }
    }

    private static BonkedModule? instance;

    private List<BonkedMember> bonkedMembers;
    private ulong bonkedRole => 967594914630217798;
    private ulong humanRole => 967584069430956072;

    public BonkedModule()
    {
        bonkedMembers = new List<BonkedMember>();
    }

    public async Task AddBonkedMember(ulong id, double bonkTimeMinutes, SocketCommandContext context)
    {
        var guild = context.Guild;
        var guildUser = guild.GetUser(id);
        var allRoles = guildUser.Roles.ToList();
        allRoles.RemoveAt(0);

        foreach (var role in allRoles) {
            await guildUser.RemoveRoleAsync(role.Id);
        }
        await guildUser.AddRoleAsync(bonkedRole);

        var bonked = new BonkedMember(id, MinuteToMilliseconds(bonkTimeMinutes), allRoles);

        bool runCheck = bonkedMembers.Count == 0;

        var exist = bonkedMembers.Find(x => x.id == id);
        if (exist != null) {
            exist.bonkDuration += bonkTimeMinutes;
        }
        else {
            bonkedMembers.Add(bonked);
        }

        if (runCheck) {
            CheckMembers(context);
        }

        await context.Channel.SendMessageAsync($"Bonked {guildUser.Username} for {bonkTimeMinutes.ToString("0.0")} Minutes");
    }

    public async Task UnbonkMember(ulong id, SocketCommandContext context)
    {
        var guild = context.Guild;
        var guildUser = guild.GetUser(id);
        var bonkedMember = bonkedMembers.Find(x => x.id == id);
        if (bonkedMember == null) {
            return;
        }
        
        foreach (var role in bonkedMember.initialRoles) {
            await guildUser.AddRoleAsync(role);
        }
        await guildUser.RemoveRoleAsync(bonkedRole);

        RemoveBonkedMember(id);
    }
    
    public async Task UnbonkMember(BonkedMember id, SocketCommandContext context)
    {
        var guild = context.Guild;
        var guildUser = guild.GetUser(id.id);
        await guildUser.AddRolesAsync(id.initialRoles);
        await guildUser.RemoveRoleAsync(bonkedRole);

        RemoveBonkedMember(id.id);
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
                var endTime = member.startTime.TimeOfDay.TotalMilliseconds + member.bonkDuration;

                if (DateTime.Now.TimeOfDay.TotalMilliseconds > endTime) {
                    await UnbonkMember(member, context);
                }
            }

            await Task.Delay(100);
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
        this.initialRoles = initialRoles;
    }

    public List<SocketRole> initialRoles;
    public ulong id;
    public DateTime startTime;
    public double bonkDuration; // in milliseconds
}