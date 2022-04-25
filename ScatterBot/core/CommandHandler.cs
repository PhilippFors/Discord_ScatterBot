using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Helpers;
using ScatterBot.core.Modules;

namespace ScatterBot.core;

public class CommandHandler
{
    private readonly DiscordSocketClient client;
    private readonly CommandService commands;

    public CommandHandler(DiscordSocketClient client, CommandService commands)
    {
        this.client = client;
        this.commands = commands;
    }

    public async Task InstallCommandsAsync()
    {
        client.MessageReceived += HandleCommandAsync;
        client.MessageUpdated += HandleMsgUpdate;

        await commands.AddModulesAsync(
            assembly: Assembly.GetEntryAssembly(),
            services: null);
    }

    private async Task HandleMsgUpdate(Cacheable<IMessage,ulong> messages, SocketMessage m, ISocketMessageChannel channel)
    {
        await PinHelper.Instance.Pin(m, channel, client);
    }
    
    private async Task HandleCommandAsync(SocketMessage socketMessage)
    {
        var message = socketMessage as SocketUserMessage;
        if (message == null) {
            return;
        }

        var guild = client.GetGuild(967532421975273563);
        var user = guild.GetUser(message.Author.Id);
        var perms = user.GuildPermissions;
        
        if(!perms.Has(GuildPermission.ModerateMembers) || message.Author.IsBot)
        {
            return;
        }

        int argPos = 0;
        
        if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) {
            return;
        }
        
        var context = new SocketCommandContext(client, message);
        await commands.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: null);
    }
}