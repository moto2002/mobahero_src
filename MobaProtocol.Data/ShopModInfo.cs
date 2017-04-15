using ProtoBuf;
using System;
using System.Collections;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ShopModInfo
	{
		[ProtoMember(1)]
		public int id;

		[ProtoMember(2)]
		public long fromTime
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long toTime
		{
			get;
			set;
		}

		public Hashtable modTable
		{
			get;
			set;
		}
	}
}
