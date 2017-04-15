using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class EnchantData
	{
		[ProtoMember(1)]
		public long Epid
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
	}
}
