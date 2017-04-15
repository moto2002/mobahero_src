using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CAddMoney
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
		public int currMoney
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public byte addType
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int deadId
		{
			get;
			set;
		}
	}
}
