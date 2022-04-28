using System.Collections.Generic;
using ProtoBuf;

namespace ScatterBot_v2.core.Serialization;

[ProtoContract]
public class ServerData
{
    [ProtoMember(1)] public ulong guildId { get; set; }

    [ProtoMember(2)] public ulong welcomeChannel { get; set; }

    [ProtoMember(3)] public ulong botLogChannel { get; set; }

    [ProtoMember(4)] public ulong mutedRoleId { get; set; }

    [ProtoMember(5)] public ulong accessRoleId { get; set; }

    [ProtoMember(6)] public List<MessageSaveData> newIntroductions { get; set; }
    [ProtoMember(7)] public List<BonkedMember> bonkedMembers { get; set; }
    [ProtoMember(8)] public List<ulong> newUsers { get; set; }
}

[ProtoContract]
public class BonkedMember
{
    [ProtoMember(1)] public List<ulong> initialRoles;
    [ProtoMember(2)] public ulong id;
    [ProtoMember(3)] public double startTime;
    [ProtoMember(4)] public double endTime;
    [ProtoMember(5)] public double bonkDuration; // in seconds
}

[ProtoContract]
public class MessageSaveData
{
    [ProtoMember(1)] public ulong channel { get; set; }
    [ProtoMember(2)] public ulong messageId { get; set; }
    [ProtoMember(3)] public ulong userId { get; set; }
}