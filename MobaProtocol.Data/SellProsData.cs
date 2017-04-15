using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SellProsData
	{
		[ProtoMember(1)]
		public int EpId
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
