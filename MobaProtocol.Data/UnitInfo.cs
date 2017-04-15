using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UnitInfo
	{
		[ProtoMember(6)]
		public int controlPlayerNewUid;

		[ProtoMember(7)]
		public string burnValue;

		[ProtoMember(8)]
		public UnitType unitType;

		[ProtoMember(9)]
		public byte lifeState;

		[ProtoMember(10)]
		public HeroRunTimeInfo heroInfo;

		[ProtoMember(11)]
		public MapItemInfo skillunitInfo;

		[ProtoMember(12)]
		public float liveTime;

		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string typeId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int creepId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int level
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public byte group
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public int monsterTeamId
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public SVector3 position
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public int mainHeroId
		{
			get;
			set;
		}
	}
}
