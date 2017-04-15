using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class BattleRoomInfo
	{
		[ProtoMember(1)]
		public int roomId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int groupPlayerCount
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int battleId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public byte gameType
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string roomGid
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public bool isSelfDefRoom
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string roomVoiceID
		{
			get;
			set;
		}
	}
}
