using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ShopUseItemInfo
	{
		[ProtoMember(1)]
		public int itemoid
		{
			get;
			set;
		}
	}
}
