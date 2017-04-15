using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KDAData
	{
		[ProtoMember(1)]
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string name
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int saygood
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int wincount
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int losecount
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int killcount
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int deathcount
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int assistantcount
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int triplekill
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int quadrakill
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public int pentakill
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public int godlike
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public HeroUsedData[] herouseinfos
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int ladderscore
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public BattleInfoData[] battleinfos
		{
			get;
			set;
		}
	}
}
