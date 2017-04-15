using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class C2PPause
	{
		[ProtoMember(1)]
		public bool pause
		{
			get;
			set;
		}
	}
}
