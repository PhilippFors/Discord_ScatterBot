﻿using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;

namespace ScatterBot_v2.core.ErrorHandling
{
    public class ErrorHandler
    {
        private Services service;
        private DiscordClient client;

        public ErrorHandler(Services service, DiscordClient client)
        {
            this.service = service;
            this.client = client;
        }

        public async Task HandleCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            var commandName = e.Command.Name;
            var member = e.Context.Member;
            string message = "";

            switch (e.Exception)
            {
                case CommandNotFoundException:
                    message = $"{member.Mention} {commandName} not found.";
                    break;
                case ChecksFailedException:
                    message = $"{member.Mention} has no permission to use {commandName}.";
                    break;
                default:
                    message = $"{commandName} failed with exception \"{e.Exception}\"";
                    break;
            }
        
            await e.Context.RespondAsync(message);
        }
    }
}