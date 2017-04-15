using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CActiveUnit
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}
	}
}
