using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace ScatterBot_v2.core.Extensions;

public static class MessageExtensions
{
    public static async Task WaitDeleteMessage(this DiscordMessage m, double time = 2)
    {
        await Task.Delay(TimeSpan.FromSeconds(time));
        await m.DeleteAsync();
    }
}