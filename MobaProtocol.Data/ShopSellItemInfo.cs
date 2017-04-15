using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ShopSellItemInfo
	{
		[ProtoMember(1)]
		public string shopId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int itemoid
		{
			get;
			set;
		}
	}
}
