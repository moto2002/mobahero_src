using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ShopData
	{
		[ProtoMember(1)]
		public int ShopType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<ShopGoodsModel> List
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int DayRestCount
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public long TimeSceondLeft
		{
			get;
			set;
		}
	}
}
