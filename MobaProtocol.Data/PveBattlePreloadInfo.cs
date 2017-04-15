using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PveBattlePreloadInfo
	{
		[ProtoMember(1)]
		public BattleRoomInfo battleInfo
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<PvePlayerInfo> lmList
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public List<PvePlayerInfo> blList
		{
			get;
			set;
		}
	}
}
