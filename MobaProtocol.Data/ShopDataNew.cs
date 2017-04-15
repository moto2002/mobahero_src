using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ShopDataNew
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
		public string Picture
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string Name
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int WeightFactor
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string GoodsId
		{
			get;
			set;
		}
	}
}
