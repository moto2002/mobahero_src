using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CMonsterPseudoDeath
	{
		[ProtoMember(1)]
		public int attackerId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int newGroup
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public float hp
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public float mp
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string typeId
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int oldCreepVoId
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int oldGroup
		{
			get;
			set;
		}
	}
}
