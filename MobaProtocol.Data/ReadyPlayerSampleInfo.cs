using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ReadyPlayerSampleInfo
	{
		[ProtoMember(1)]
		public int newUid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int serverId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public byte group
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public byte teamPos
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string userName
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public HeroInfo heroInfo
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public bool readyChecked
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public bool horeSelected
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int level
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public string selfDefSkillId
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public string heroSkinId
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public long SummerId
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public bool IsReadySelectHero
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int RankFrame
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public int CharmRankvalue
		{
			get;
			set;
		}
	}
}
