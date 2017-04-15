using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HerosPositionData
	{
		[ProtoMember(1)]
		public string ModelId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Position
		{
			get;
			set;
		}
	}
}
