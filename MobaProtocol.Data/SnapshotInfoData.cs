using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SnapshotInfoData
	{
		[ProtoMember(1)]
		public float CurHp
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public float MaxHp
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public float Dps
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string ModelId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public bool IsHero
		{
			get;
			set;
		}
	}
}
