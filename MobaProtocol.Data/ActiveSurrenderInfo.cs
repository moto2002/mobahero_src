using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ActiveSurrenderInfo
	{
		[ProtoContract]
		public class VoteInfo
		{
			[ProtoMember(1)]
			public int userId;

			[ProtoMember(2)]
			public bool accept;
		}

		[ProtoMember(1)]
		public int starterUid;

		[ProtoMember(2)]
		public DateTime startTime = default(DateTime);

		[ProtoMember(3)]
		public List<ActiveSurrenderInfo.VoteInfo> votes = new List<ActiveSurrenderInfo.VoteInfo>();

		[ProtoMember(4)]
		public List<int> validVoters = new List<int>();
	}
}
