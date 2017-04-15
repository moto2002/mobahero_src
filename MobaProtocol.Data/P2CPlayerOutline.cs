using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CPlayerOutline
	{
		[ProtoMember(1)]
		public int newUid
		{
			get;
			set;
		}
	}
}
