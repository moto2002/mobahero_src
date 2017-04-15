using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class C2PPveReconnect
	{
		[ProtoMember(1)]
		public int roomId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string key
		{
			get;
			set;
		}
	}
}
