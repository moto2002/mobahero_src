using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class InBattleRoomInfo
	{
		[ProtoMember(1)]
		public BattleRoomInfo roomInfo
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public ReadyPlayerSampleInfo[] playerInfos
		{
			get;
			set;
		}
	}
}
