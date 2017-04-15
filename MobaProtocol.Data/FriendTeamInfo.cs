using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class FriendTeamInfo
	{
		[ProtoMember(1)]
		public string teamId;

		[ProtoMember(2)]
		public PvpMemberInfo[] members;
	}
}
