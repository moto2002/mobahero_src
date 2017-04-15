using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ShopGoodsModel
	{
		[ProtoMember(1)]
		public int PropsId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Count
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int PriceSum
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public bool IsBuyed
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public byte MoneyType
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int OnlyId
		{
			get;
			set;
		}
	}
}
