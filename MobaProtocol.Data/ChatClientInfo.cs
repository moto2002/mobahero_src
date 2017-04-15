using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ChatClientInfo
	{
		[ProtoMember(1)]
		public string NickName
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string ServerId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public bool IsRoom
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int RoomId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int UnionId
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string UserId
		{
			get;
			set;
		}
	}
}
