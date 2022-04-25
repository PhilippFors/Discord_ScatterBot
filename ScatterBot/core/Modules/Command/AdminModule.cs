using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.Command;

[Group("admin")]
[RequireUserPermission(GuildPermission.ModerateMembers)]
public class AdminModule : ModuleBase<SocketCommandContext>
{
    [Command("ban")]
    [Summary("Bans a user.")]
    public async Task BanAsync(SocketUser user, [Remainder] string reason = "")
    {
        await Context.Guild.AddBanAsync(user, reason: reason);
        await Context.LogToChannel($"{user.Username} has been banned.");
    }

    [Command("ban")]
    [Summary("Bans a user.")]
    public async Task BanAsync(ulong id, [Remainder] string reason = "")
    {
        await Context.Guild.AddBanAsync(id, reason: reason);
        await Context.LogToChannel($"{id} has been banned.");
    }

    [Command("bonk")]
    [Summary("Takes away all roles and assigns the 'bonked' role.")]
    public async Task Bonk(ulong id, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(id, time, Context);
    }

    [Command("bonk")]
    [Summary("Takes away all roles and assigns the 'bonked' role.")]
    public async Task Bonk(SocketUser user, double time = 1)
    {
        await BonkedHelper.Instance.AddBonkedMember(user.Id, time, Context);
    }

    [Command("unbonk")]
    [Summary("Resets all roles of user to their original roles.")]
    public async Task UnBonk(ulong id)
    {
        await BonkedHelper.Instance.UnbonkMember(id, Context.Client);
    }

    [Command("unbonk")]
    [Summary("Resets all roles of user to their original roles.")]
    public async Task UnBonk(SocketUser user)
    {
        await BonkedHelper.Instance.UnbonkMember(user.Id, Context.Client);
    }
}