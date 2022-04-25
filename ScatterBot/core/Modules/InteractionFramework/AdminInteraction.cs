using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.InteractionFramework;

[Group("admin", "Admin tools")]
[RequireUserPermission(GuildPermission.ModerateMembers)]
public class AdminInteraction : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ban_username", "bans user")]
    public async Task BanAsync(SocketUser user, string reason = "")
    {
        await Context.Guild.AddBanAsync(user, reason: reason);
        await Context.Client.LogToChannel($"{user.Username} has been banned.");
    }

    [SlashCommand("ban_id", "bans user")]
    public async Task BanAsync(ulong id, string reason = "")
    {
        await Context.Guild.AddBanAsync(id, reason: reason);
        await Context.Client.LogToChannel($"{id} has been banned.");
    }

    [SlashCommand("bonk_id", "bonks user")]
    public async Task Bonk(ulong id, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(id, time, Context);
    }

    [SlashCommand("bonk_username", "bonks user")]
    public async Task Bonk(SocketUser user, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(user.Id, time, Context);
    }

    [SlashCommand("unbonk_id", "unbonk user")]
    public async Task UnBonk(ulong id)
    {
        await BonkedHelper.Instance.UnbonkMember(id, Context.Client);
    }
    
    [SlashCommand("unbonk_user", "unbonk user")]
    public async Task UnBonk(SocketUser user)
    {
        await BonkedHelper.Instance.UnbonkMember(user.Id, Context.Client);
    }
}