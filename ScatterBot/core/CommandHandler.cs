using System.Formats.Asn1;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using ScatterBot.core.Helpers;

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
        client.MessageReceived += HandleBonkedMessages;
        client.MessageUpdated += HandleMsgUpdate;
        client.UserJoined += ctx => RoleAssignHelper.Instance.AddUser(ctx.Id, ctx.Guild);
        
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

        int argPos = 0;
        
        if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)) || message.Author.IsBot) {
            return;
        }
        
        var context = new SocketCommandContext(client, message);
        await commands.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: null);
    }

    private async Task HandleBonkedMessages(SocketMessage message)
    {
        if (message.Channel.Id != HardcodedShit.welcomeId) {
            return;
        }
        
        var author = message.Author;
        if (BonkedHelper.Instance.IsBonked(author.Id)) {
            var m = await message.Channel.SendMessageAsync($"No talking for you {author.Mention}.", messageReference: message.Reference);
            await message.DeleteAsync();
            WaitDeleteMessage(m);
        }
    }

    private async Task WaitDeleteMessage(RestUserMessage m)
    {
        await Task.Delay(TimeSpan.FromSeconds(15));
        await m.DeleteAsync();
    }
}