using System.Linq;
using DSharpPlus.Entities;

namespace ScatterBot_v2.core.Extensions
{
    public static class DiscordMemberExtensions
    {
        public static bool HasRole(this DiscordMember user, ulong roleId)
        {
            return user.Roles.ToList().Exists(x => x.Id == roleId);
        }
    
        public static bool HasRole(this DiscordMember user, string roleName)
        {
            return user.Roles.ToList().Exists(x => x.Name == roleName);
        }
    }
}