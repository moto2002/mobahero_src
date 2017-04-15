using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ShopGoodsCost
	{
		[ProtoMember(1)]
		public byte costType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int priceSum
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public float discount
		{
			get;
			set;
		}
	}
}
