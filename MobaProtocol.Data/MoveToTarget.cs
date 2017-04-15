using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MoveToTarget
	{
		[ProtoMember(1)]
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

		[ProtoMember(2)]
		public SVector3 pos
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public short _targetId
		{
			get;
			set;
		}

		public int targetId
		{
			get
			{
				return (int)this._targetId;
			}
			set
			{
				this._targetId = (short)value;
			}
		}

		[ProtoMember(4)]
		public SVector3 targetPos
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public float stopDis
		{
			get;
			set;
		}
	}
}
