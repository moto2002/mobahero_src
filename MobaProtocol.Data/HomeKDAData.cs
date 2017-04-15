using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HomeKDAData
	{
		[ProtoMember(1)]
		public int wincount
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int losecount
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public BattleInfoData[] battleinfos
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int killcount
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int deathcount
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int assistantcount
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int mvp
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int godlike
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int pentakill
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
		public int triplekill
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public int doublekill
		{
			get;
			set;
		}
	}
}
