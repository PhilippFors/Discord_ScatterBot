using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules;

[Group("admin")]
[RequireUserPermission(GuildPermission.ModerateMembers)]
public class AdminModule : ModuleBase<SocketCommandContext>
{
    [Command("ban")]
    public async Task BanAsync(SocketUser user, [Remainder] string reason = "")
    {
        await Context.Guild.AddBanAsync(user, reason: reason);
        await Context.LogToChannel($"{user.Username} has been banned.");
    }

    [Command("ban")]
    public async Task BanAsync(ulong id, [Remainder] string reason = "")
    {
        await Context.Guild.AddBanAsync(id, reason: reason);
        await Context.LogToChannel($"{id} has been banned.");
    }

    [Command("bonk")]
    public async Task Bonk(ulong id, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(id, time, Context);
    }

    [Command("bonk")]
    public async Task Bonk(SocketUser user, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(user.Id, time, Context);
    }

    [Command("unbonk")]
    public async Task UnBonk(ulong id)
    {
        await BonkedHelper.Instance.UnbonkMember(id, Context);
    }

    [Command("unbonk")]
    public async Task UnBonk(SocketUser user)
    {
        await BonkedHelper.Instance.UnbonkMember(user.Id, Context);
    }
}