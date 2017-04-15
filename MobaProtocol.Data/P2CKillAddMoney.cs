using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CKillAddMoney
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
		public int deadId
		{
			get;
			set;
		}
	}
}
