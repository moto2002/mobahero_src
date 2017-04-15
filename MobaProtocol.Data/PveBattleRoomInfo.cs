using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PveBattleRoomInfo
	{
		[ProtoMember(1)]
		public int roomId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int battleId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string roomGid
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string userId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string ip
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int port
		{
			get;
			set;
		}
	}
}
