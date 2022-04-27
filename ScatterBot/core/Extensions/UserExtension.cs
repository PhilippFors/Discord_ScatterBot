using Discord.WebSocket;

namespace ScatterBot.core.Extensions;

public static class UserExtension
{
    
    public static bool HasRole(this SocketGuildUser user, ulong roleid)
    {
        return user.Roles.ToList().Exists(x => x.Id == roleid);
    }
    
    public static bool HasRole(this SocketGuildUser user, string roleName)
    {
        return user.Roles.ToList().Exists(x => x.Name == roleName);
    }
}