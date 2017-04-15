using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SkillDynData
	{
		[ProtoMember(1)]
		public byte level
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long cd
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public byte layer
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public float chargeCD
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public byte useState
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int skillIdx
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int index
		{
			get;
			set;
		}
	}
}
