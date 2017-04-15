using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ChatMessagePullData
	{
		[ProtoMember(1)]
		public List<ChatMessageNew> messages
		{
			get;
			set;
		}
	}
}
