using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class LoginInfo
	{
		[ProtoMember(1)]
		public string accountid;

		[ProtoMember(2)]
		public int serverid;

		[ProtoMember(3)]
		public int type;
	}
}
