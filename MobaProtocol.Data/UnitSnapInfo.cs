using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UnitSnapInfo
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
		public SVector3 srcPos
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public SVector3 pos
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public MoveState state
		{
			get;
			set;
		}
	}
}
