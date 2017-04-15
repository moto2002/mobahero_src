using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TeamReadyPlayerInfo
	{
		[ProtoMember(1)]
		public List<ReadyPlayerInfo> players;
	}
}
