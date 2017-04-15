using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UnitRuntimeInfo
	{
		[ProtoMember(8)]
		public Dictionary<string, BuffRuntimeData> buffInfo;

		[ProtoMember(9)]
		public sbyte[] nVisebleState;

		[ProtoMember(1)]
		public SVector3 position
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public float rotateY
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public float hp
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public float mp
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public long reliveLeftTime
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public UnitInfo baseUnitInfo
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public SVector3 targetPosition
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public float maxhp
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public float maxmp
		{
			get;
			set;
		}
	}
}
