using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ProbNull
	{
		[ProtoMember(1)]
		public int nil
		{
			get;
			set;
		}
	}
}
