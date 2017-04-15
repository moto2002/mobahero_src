using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DataChangeInfo
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int attackerId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int[] damageIds
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public bool reverse
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public float[] values
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string buffId
		{
			get;
			set;
		}
	}
}
