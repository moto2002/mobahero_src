using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class GoodsData
	{
		[ProtoMember(1)]
		public int Id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Type
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string ElementId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int Count
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string Price
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int BuyMaxNum
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int DayBuyCount
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int IsSingle
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int WeightFactor
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int Mark
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public string Picture
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public ShopModInfo modInfo
		{
			get;
			set;
		}
	}
}
