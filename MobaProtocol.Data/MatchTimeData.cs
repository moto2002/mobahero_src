using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MatchTimeData
	{
		[ProtoMember(1)]
		public int MatchId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public DateTime BeginTime
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public DateTime EndTime
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<MatchRankInfo> list
		{
			get;
			set;
		}
	}
}
