using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class JumpFontInfo
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int type
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string text
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int attackerId
		{
			get;
			set;
		}
	}
}
