using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NewbieStartGameData
	{
		[ProtoMember(1)]
		public PvpStartGameInfo startGameInfo
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public BattleRoomInfo btRoomInfo
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public ReadyPlayerSampleInfo[] playerInfos
		{
			get;
			set;
		}
	}
}
