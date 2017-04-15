using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CAddExp
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int add
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int currExp
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int level
		{
			get;
			set;
		}
	}
}
