using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MatchRankInfo
	{
		[ProtoMember(1)]
		public string SumName
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public double Points
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Rk
		{
			get;
			set;
		}
	}
}
