using Discord.Commands;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Modules.Command;

public class Utility : ModuleBase<SocketCommandContext>
{
    // [Command()]
    // public async Task CheckMember()
    // {
    //     if (Context.Channel.Id == HardcodedShit.welcomeId &&
    //         BonkedHelper.Instance.IsBonked(Context.Message.Author.Id)) {
    //         await Context.Message.DeleteAsync();
    //         await Context.Channel.SendMessageAsync($"No talking for you {Context.Message.Author.Mention}.");
    //     }
    // }
}