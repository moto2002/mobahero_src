using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ChatUserInfo
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long chatCheckTime
		{
			get;
			set;
		}
	}
}
