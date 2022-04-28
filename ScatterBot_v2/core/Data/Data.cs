using System.Collections.Generic;
using ScatterBot_v2.core.Serialization;

namespace ScatterBot_v2.core.Data;

public static class Roles
{
    public static ulong mutedRoleId;

    public static ulong accessRoleId;

    public static ulong[] allRoleIds;

    public static string[] allRoleNames;
}

public static class Channels
{
    public static ulong logChannelId;

    public static ulong welcomeChannelId;

    public static ulong[] allChannelIds;

    public static string[] allChannelNames;
}

public static class Moderation
{
    public static List<BonkedMember> bonkedMembers;

    public static List<MessageSaveData> newIntroductions;

    public static List<ulong> newUsers;

    public static ulong philUserId = 173193609795862529; // hard coding an admin (me)
}

public static class Guild
{
    public static ulong guildId = 967532421975273563; // like the only hardcoded thing
}