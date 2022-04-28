﻿using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ScatterBot_v2.core.Serialization;

namespace ScatterBot_v2.core.Modules.TextBasedCommands;

[Group("init")]
[RequirePermissions(Permissions.ModerateMembers)]
public class InitializeTextCommands : BaseCommandModule
{
    public SaveSystem saveSytem { private get; set; }
    
    [Command("welcome")]
    public async Task InitWelcomeChannel(CommandContext context, ulong channelId)
    {
        saveSytem.ServerData.welcomeChannel = channelId;
    }

    [Command("botlog")]
    public async Task InitBotLogChannel(CommandContext context, ulong channelId)
    {
        saveSytem.ServerData.botLogChannel = channelId;
    }

    [Command("muted")]
    public async Task InitWelcomeChannel(CommandContext context, DiscordRole mutedRole)
    {
        saveSytem.ServerData.mutedRoleId = mutedRole.Id;
    }

    [Command("access")]
    public async Task InitBotLogChannel(CommandContext context, DiscordRole accessRole)
    {
        saveSytem.ServerData.accessRoleId = accessRole.Id;
    }

    [Command("save")]
    public Task Save(CommandContext context)
    {
        return saveSytem.SaveData();
    }

    [Command("query")]
    public async Task Query(CommandContext context)
    {
        await context.RespondAsync($"{saveSytem.ServerData.welcomeChannel}, {saveSytem.ServerData.botLogChannel}");
    }
}