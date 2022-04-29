using DSharpPlus.Entities;

namespace ScatterBot_v2.Data
{
    public static class Roles
    {
        public static ulong[] allRoleIds;

        public static string[] allRoleNames;
    }

    public static class Channels
    {
        public static ulong[] allChannelIds;

        public static string[] allChannelNames;
    }

    public static class Guild
    {
        public static ulong guildId = 967532421975273563; // like the only hardcoded thing
        public static DiscordGuild guild;
    }
}