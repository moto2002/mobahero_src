using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TeamPvpData
	{
		[ProtoMember(1)]
		public Dictionary<TeamRoomType, FriendTeamInfo> teamData;
	}
}
