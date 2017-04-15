using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HistoryBattleData
	{
		[ProtoMember(1)]
		public string battleid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public bool win
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public List<MemberBattleInfo> selfteaminfo
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<MemberBattleInfo> enemyteaminfo
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public List<GroupTeamInfo> teamInfos
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public DateTime timestart
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public DateTime timend
		{
			get;
			set;
		}
	}
}
