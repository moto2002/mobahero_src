using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CAddItem
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public ItemDynData data
		{
			get;
			set;
		}
	}
}
