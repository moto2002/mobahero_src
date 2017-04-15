using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ChatMessageNew
	{
		[ProtoMember(1)]
		public AgentBaseInfo Client
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string Message
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long TimeTick
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string TargetId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public byte ChatType
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public long MessageId
		{
			get;
			set;
		}
	}
}
