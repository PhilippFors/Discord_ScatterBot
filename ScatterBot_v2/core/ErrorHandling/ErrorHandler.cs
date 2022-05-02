using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using Serilog;

namespace ScatterBot_v2.core.ErrorHandling
{
    /// <summary>
    /// Responds with an error message and logs exceptions to a file.
    /// </summary>
    public static class ErrorHandler
    {
        public static async Task HandleCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            var commandName = e.Command.Name;
            var member = e.Context.Member;
            string message;

            switch (e.Exception)
            {
                case CommandNotFoundException:
                    message = $"{member.Mention} {commandName} not found.";
                    break;
                case ChecksFailedException:
                    message = $"{member.Mention} has no permission to use {commandName}.";
                    break;
                default:
                    message = $"{member.Mention}, {commandName} failed with an exception: \"{e.Exception.Message}\". Check logs for more details.";
                    Log.Logger.Error(e.Exception, "Something went wrong with a command");
                    break;
            }
        
            await e.Context.RespondAsync(message);
        }
    }
}