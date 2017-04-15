using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MagicBottleRankList
	{
		[ProtoMember(1)]
		public List<MagicBottleRankData> list
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int myrank
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long todayexp
		{
			get;
			set;
		}
	}
}
