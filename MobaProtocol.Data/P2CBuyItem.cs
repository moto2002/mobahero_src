using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CBuyItem
	{
		[ProtoMember(1)]
		public int itemoid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte retaCode
		{
			get;
			set;
		}
	}
}
