using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace ScatterBot.core.Extensions;

public static class ChannelLoggingExtenstion
{
    public static async Task LogToChannel(this SocketCommandContext context, string msg)
    {
        var botlogschannel = context.GetChannel(HardcodedShit.botLogsId);
        await botlogschannel.SendMessageAsync(msg);
    }

    public static async Task LogToChannel(this DiscordSocketClient context, string msg)
    {
        var botlogschannel = context.GetChannel(HardcodedShit.botLogsId);
        var channel = botlogschannel as IMessageChannel;
        await channel.SendMessageAsync(msg);
    }

    public static async Task LogToChannel(this IDiscordClient context, string msg)
    {
        var botlogschannel = await context.GetChannelAsync(HardcodedShit.botLogsId);
        var channel = botlogschannel as IMessageChannel;
        await channel.SendMessageAsync(msg);
    }

    public static async Task LogToChannel(this SocketInteractionContext context, string msg)
    {
        var botlogschannel = await context.Client.GetChannelAsync(HardcodedShit.botLogsId);
        var channel = botlogschannel as IMessageChannel;
        await channel.SendMessageAsync(msg);
    }
}