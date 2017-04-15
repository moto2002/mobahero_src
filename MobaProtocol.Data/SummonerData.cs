using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SummonerData
	{
		[ProtoMember(1)]
		public int SummonerId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long TalentId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string TalentCount
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string UsedTalentCount
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string summSkills
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int Charm
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int HighScore
		{
			get;
			set;
		}
	}
}
