using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CGetMapItem
	{
		[ProtoMember(1)]
		public int attackerId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int targetId
		{
			get;
			set;
		}
	}
}
