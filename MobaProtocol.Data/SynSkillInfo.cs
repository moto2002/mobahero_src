using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SynSkillInfo
	{
		[ProtoMember(1)]
		public short unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string skillId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int extraInt1
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public float extraInt2
		{
			get;
			set;
		}
	}
}
