using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class C2PQueryUnit
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}
	}
}
