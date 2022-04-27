using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ScatterBot_v2.core.Helpers;

namespace ScatterBot_v2.core.Modules.InteractionFramework;

// [Group("_admin")]
// [RequirePermissions(Permissions.ModerateMembers)]
public class MemberManagementInteractionCommands : ApplicationCommandModule
{
    // [Group("_access")]
    // [RequirePermissions(Permissions.ModerateMembers)]
    // public class ServerAccess : BaseCommandModule
    // {
    //     [SlashCommand("new_users", "Assigns the 'possibly human' role to new users."), Description("does things")]
    //     public async Task AssignRolesToNewUser(CommandContext context)
    //     {
    //         // await NewUserHelper.Instance.AssignRoles(Context.Client);
    //     }
    //
    //     [SlashCommand("automate_access", "Enables automatic accepting of people who sent introduction in #welcome.")]
    //     public async Task AutomateWelcome()
    //     {
    //         NewUserHelper.Instance.StartAutomateUserWelcome();
    //     }
    //
    //     [SlashCommand("stop_automate_access",
    //         "Disables automatic accepting of people who sent introduction in #welcome.")]
    //     public async Task StopAutomateWelcome()
    //     {
    //         NewUserHelper.Instance.StopAutomateUserWelcome();
    //     }
    //
    //     [SlashCommand("ban_username", "bans user with username")]
    //     public async Task BanAsync(CommandContext context, DiscordMember user, int time = 0, string reason = "")
    //     {
    //         await context.RespondAsync("this works!");
    //         // await Context.Guild.AddBanAsync(user, time, reason);
    //         // await Context.Client.LogToChannel($"{user.Username} has been banned.");
    //     }
    //
    //     [SlashCommand("ban_id", "bans user with id")]
    //     public async Task BanAsync(CommandContext context, ulong id, int time = 0, string reason = "")
    //     {
    //         return;
    //         // await Context.Guild.AddBanAsync(id, time, reason);
    //         // await Context.Client.LogToChannel($"{id} has been banned.");
    //     }
    // }
    //
    // [SlashCommand("bonk_id", "bonks user with id")]
    // public async Task Bonk(CommandContext context, ulong id, double time = 1)
    // {
    //     // await BonkedHelper.Instance.AddBonkedMember(id, time, context.Client);
    // }
    //
    // [SlashCommand("bonk_username", "bonks user by mention")]
    // public async Task Bonk(CommandContext context, DiscordMember user, double time = 1)
    // {
    //     // await BonkedHelper.Instance.AddBonkedMember(user.Id, time, Context.Client);
    // }
    //
    // [SlashCommand("unbonk_id", "unbonk user")]
    // public async Task UnBonk(CommandContext context, ulong id)
    // {
    //     // await BonkedHelper.Instance.UnbonkMember(id, Context.Client);
    // }
    //
    // [SlashCommand("unbonk_user", "unbonk user")]
    // public async Task UnBonk(CommandContext context, DiscordMember user)
    // {
    //     // await BonkedHelper.Instance.UnbonkMember(user.Id, Context.Client);
    // }
    //
    // [SlashCommand("add_role", "adds role to user")]
    // public async Task GrantRole(CommandContext context, DiscordRole roleName, DiscordMember user)
    // {
    //     // var guildUser = Context.Guild.GetUser(user.Id);
    //     // await guildUser.AddRoleAsync(roleName);
    // }
    //
    // [SlashCommand("remove_role", "removes role from user")]
    // public async Task RemoveRole(CommandContext context, DiscordRole roleName, DiscordMember user)
    // {
    //     // var guildUser = Context.Guild.GetUser(user.Id);
    //     // await guildUser.RemoveRoleAsync(roleName);
    // }
}