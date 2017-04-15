using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CUnitsVisiableChanged
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int state
		{
			get;
			set;
		}
	}
}
