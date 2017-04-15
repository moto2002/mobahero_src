using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class GroupTeamInfo
	{
		[ProtoMember(1)]
		public byte group
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int exp
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Level
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int teamKill
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int deadCount
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int helpKillCount
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int killTowerCount
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int killSpeMonsterCount
		{
			get;
			set;
		}
	}
}
