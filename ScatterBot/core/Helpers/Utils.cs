﻿using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace ScatterBot.core.Helpers;

public static class Utils
{
    public static IMessageChannel GetChannel(this SocketInteractionContext context, ulong id)
    {
        var channel = context.Guild.GetChannel(id);
        return channel as IMessageChannel;
    }

    public static IMessageChannel GetChannel(this SocketInteractionContext context, string channelId)
    {
        IMessageChannel channel;
        if (channelId != "") {
            var c = context.Guild.Channels.Where(x => x.Name == channelId).ToList();
            if (c.Count == 0) {
                return null;
            }

            channel = c[0] as IMessageChannel;
        }
        else {
            channel = context.Channel;
        }

        return channel;
    }

    public static async Task<IMessageChannel> GetChannel(this IDiscordClient context, ulong id)
    {
        var guild = await context.GetGuildAsync(HardcodedShit.guildId);
        var channel = await guild.GetChannelAsync(id);
        return channel as IMessageChannel;
    }

    public static async Task<IMessageChannel> GetChannel(this IDiscordClient context, string channelId)
    {
        var guild = await context.GetGuildAsync(HardcodedShit.guildId);
        IMessageChannel channel = null;
        if (channelId != "") {
            var c = guild.GetChannelsAsync().Result.Where(x => x.Name == channelId).ToList();
            if (c.Count == 0) {
                return null;
            }

            channel = c[0] as IMessageChannel;
        }

        return channel;
    }

    public static IMessageChannel GetChannel(this SocketCommandContext context, ulong id)
    {
        var channel = context.Guild.GetChannel(id);
        return channel as IMessageChannel;
    }

    public static IMessageChannel GetChannel(this SocketCommandContext context, string channelId)
    {
        IMessageChannel channel;
        if (channelId != "") {
            var c = context.Guild.Channels.Where(x => x.Name == channelId).ToList();
            if (c.Count == 0) {
                return null;
            }

            channel = c[0] as IMessageChannel;
        }
        else {
            channel = context.Channel;
        }

        return channel;
    }

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