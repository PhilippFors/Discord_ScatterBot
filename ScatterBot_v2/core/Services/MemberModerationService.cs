using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Services
{
    /// <summary>
    /// General member moderation stuff.
    /// </summary>
    public class MemberModerationService : ISaveable
    {
        private Dictionary<ulong, int> warnings => serverData.userWarnings;
        private readonly SaveSystem saveSystem;
        private ServerData serverData;
        
        public MemberModerationService(SaveSystem saveSystem)
        {
            this.saveSystem = saveSystem;
            Load();
        }
    
        public async Task BanMember(CommandContext context, DiscordMember user, int deleteMessage, string reason)
        {
            await context.Guild.BanMemberAsync(user, deleteMessage, reason);
            if (warnings.ContainsKey(user.Id))
            {
                warnings.Remove(user.Id);
            }
            LogBan(context, user, reason);
            Save();
        }
    
        private void LogBan(CommandContext context, DiscordMember user, string msg)
        {
            var reason = string.IsNullOrEmpty(msg) ? "" : " Reason: " + msg;
            var embed = new DiscordEmbedBuilder
            {
                Title = "Member Banned",
                Description = $"{user.Mention} has been banned.{reason}",
                Color = new DiscordColor(0xFF0000)
            };
            context.RespondAsync(embed);
        }
    
        public void WarnMember(CommandContext context, DiscordMember user, string reason)
        {
            if (warnings.ContainsKey(user.Id))
            {
                warnings[user.Id]++;
            }
            else
            {
                warnings.Add(user.Id, 1);
            }
            LogWarning(context, user, reason);
            Save();
        }

        public void QueryWarn(CommandContext context, DiscordMember user)
        {
            if (!HasWarning(context, user))
            {
                return;
            }
            
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Member {user.Mention} Warned",
                Description = $"{user.Username} has {warnings[user.Id]} out of 3 warnings.",
                Color = new DiscordColor(0xFF0000)
            };
            
            context.RespondAsync(embed);
        }

        public void RemoveWarnings(CommandContext context, DiscordMember user, int warns)
        {
            if (!HasWarning(context, user))
            {
                return;
            }
            
            if (warnings[user.Id] - warns <= 0)
            {
                warnings.Remove(user.Id);
            }
            else
            {
                warnings[user.Id] -= warns;
            }
            
            var embed = new DiscordEmbedBuilder
            {
                Title = "Warning Removed",
                Description = $"Removed {warns} warnings from {user.Mention}." ,
                Color = new DiscordColor(0xFF0000)
            };
            context.RespondAsync(embed);
            Save();
        }

        private bool HasWarning(CommandContext context, DiscordMember user)
        {
            if (!warnings.ContainsKey(user.Id))
            {
                context.RespondAsync($"{user.Username} doesn't have a warning. Yet.");
                return false;
            }

            return true;
        }
        
        private void LogWarning(CommandContext context, DiscordMember user, string msg)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Member Warned",
                Description = $"{user.Mention}, this is warning number {warnings[user.Id]} out of 3. Reason: {msg}",
                Color = new DiscordColor(0xFF0000)
            };
            context.RespondAsync(embed);
        }

        public void Save()
        {
            saveSystem.SaveAs<ServerData>(serverData);
        }

        public void Load()
        {
            serverData = saveSystem.LoadAs<ServerData>();
            serverData.userWarnings ??= new Dictionary<ulong, int>();
        }
    }
}