using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class InSelectAllInfo
	{
		[ProtoMember(3)]
		public long useTime;

		[ProtoMember(4)]
		public int newUid;

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
