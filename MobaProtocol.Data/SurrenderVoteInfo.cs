using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SurrenderVoteInfo
	{
		[ProtoMember(1)]
		public bool accept = false;
	}
}
