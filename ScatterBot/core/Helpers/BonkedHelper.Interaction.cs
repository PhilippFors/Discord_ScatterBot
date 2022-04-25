using Discord;
using Discord.Interactions;

namespace ScatterBot.core.Helpers;

public partial class BonkedHelper
{
    public async Task AddBonkedMember(ulong id, double bonkTimeMinutes, SocketInteractionContext context)
    {
        var guild = context.Guild;
        var guildUser = guild.GetUser(id);
        
        if (guildUser == null) {
            await context.LogToChannel($"User with {id} could not be found");
            return;
        }
        
        await BonkInternal(guildUser, guild.Id, bonkTimeMinutes, context.Client);
    }

    public async Task UnbonkMember(ulong id, IDiscordClient context)
    {
        var bonkedMember = bonkedMembers.Find(x => x.id == id);

        if (bonkedMember == null) {
            return;
        }

        var guild = await context.GetGuildAsync(bonkedMember.guildId);
        var guildUser = await guild.GetUserAsync(bonkedMember.id);
        await guildUser.AddRolesAsync(bonkedMember.initialRoles);
        await guildUser.RemoveRoleAsync(bonkedRole);

        RemoveBonkedMember(id);

        await context.LogToChannel($"Unbonked user {guildUser.Username}");
    }
}