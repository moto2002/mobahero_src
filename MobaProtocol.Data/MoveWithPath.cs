using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MoveWithPath
	{
		[ProtoMember(1)]
		public long tick
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public short _unitId
		{
			get;
			set;
		}

		public int unitId
		{
			get
			{
				return (int)this._unitId;
			}
			set
			{
				this._unitId = (short)value;
			}
		}

		[ProtoMember(3)]
		public short _targetUnitId
		{
			get;
			set;
		}

		public int targetUnitId
		{
			get
			{
				return (int)this._targetUnitId;
			}
			set
			{
				this._targetUnitId = (short)value;
			}
		}

		[ProtoMember(4)]
		public SVector3 pos
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public SVector3 toPos
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public short _stopDistance
		{
			get;
			set;
		}

		public float stopDistance
		{
			get
			{
				return (float)this._stopDistance / 100f;
			}
			set
			{
				this._stopDistance = (short)(value * 100f);
			}
		}

		[ProtoMember(7)]
		public List<SVector3> path
		{
			get;
			set;
		}
	}
}
