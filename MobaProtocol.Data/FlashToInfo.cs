using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class FlashToInfo
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
		public short _speed
		{
			get;
			set;
		}

		public float speed
		{
			get
			{
				return (float)this._speed;
			}
			set
			{
				this._speed = (short)value;
			}
		}

		[ProtoMember(3)]
		public SVector3 pos
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public short _targUnitId
		{
			get;
			set;
		}

		public int targUnitId
		{
			get
			{
				return (int)this._targUnitId;
			}
			set
			{
				this._targUnitId = (short)value;
			}
		}

		[ProtoMember(5)]
		public float stopDist
		{
			get;
			set;
		}
	}
}
