using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ExchangeData
	{
		[ProtoMember(1)]
		public byte Type
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
		public int Rate
		{
			get;
			set;
		}
	}
}
