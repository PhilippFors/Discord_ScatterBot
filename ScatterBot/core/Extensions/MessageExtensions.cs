using Discord.Rest;

namespace ScatterBot.core.Extensions;

public static class MessageExtensions
{
    public static async Task WaitDeleteMessage(this RestUserMessage m, double time = 2)
    {
        await Task.Delay(TimeSpan.FromSeconds(time));
        await m.DeleteAsync();
    }
}