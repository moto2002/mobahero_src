using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MoveToPos
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public SVector3 pos
		{
			get;
			set;
		}
	}
}
