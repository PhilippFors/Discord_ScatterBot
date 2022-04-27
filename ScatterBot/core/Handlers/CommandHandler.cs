using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ScatterBot.core.Extensions;
using ScatterBot.core.Helpers;

namespace ScatterBot.core.Handlers;

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
        client.UserJoined += ctx => NewUserHelper.Instance.AddUser(ctx.Id, ctx.Guild);

        await commands.AddModulesAsync(
            assembly: Assembly.GetEntryAssembly(),
            services: null);
    }

    private async Task HandleMsgUpdate(Cacheable<IMessage, ulong> messages, SocketMessage m,
        ISocketMessageChannel channel)
    {
        await PinHelper.Instance.Pin(m, channel, client);
    }

    private async Task HandleCommandAsync(SocketMessage socketMessage)
    {
        if (socketMessage.Channel.Id == HardcodedShit.welcomeId) {
            // Bonked users can still type in welcome so this bonks 'em again
            var author = socketMessage.Author;
            if (BonkedHelper.Instance.IsBonked(author.Id)) {
                var response = await socketMessage.Channel.SendMessageAsync($"No talking for you {author.Mention}.");
                await socketMessage.DeleteAsync();
                response.WaitDeleteMessage(15);
            }

            // new user messages will be logged and can be used later for automatic acceptance
            NewUserHelper.Instance.AddWelcomeMessage(socketMessage, client);
        }

        var message = socketMessage as SocketUserMessage;
        if (message == null) {
            return;
        }

        int argPos = 0;

        // Check if the message has a valid command prefix
        if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
            message.Author.IsBot) {
            return;
        }

        var context = new SocketCommandContext(client, message);
        await commands.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: null);
    }
}