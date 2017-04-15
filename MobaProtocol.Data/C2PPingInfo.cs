using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class C2PPingInfo
	{
		[ProtoMember(1)]
		public long clientTime
		{
			get;
			set;
		}
	}
}
