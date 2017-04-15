using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ChatMessage
	{
		[ProtoMember(1)]
		public bool isLogin
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public ChatClientInfo Client
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string Message
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public long TimeTick
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string TargetId
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public byte ChatType
		{
			get;
			set;
		}
	}
}
