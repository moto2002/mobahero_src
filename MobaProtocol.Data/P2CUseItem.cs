using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CUseItem
	{
		[ProtoMember(1)]
		public int itemoid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte retaCode
		{
			get;
			set;
		}
	}
}
