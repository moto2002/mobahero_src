using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class AllReadyTeam
	{
		[ProtoMember(1)]
		public List<TeamReadyPlayerInfo> teams;
	}
}
