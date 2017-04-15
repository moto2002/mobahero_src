using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CPause
	{
		[ProtoMember(1)]
		public bool pause
		{
			get;
			set;
		}
	}
}
