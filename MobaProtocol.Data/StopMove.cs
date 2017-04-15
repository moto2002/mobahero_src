using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class StopMove
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
		public short _rotate
		{
			get;
			set;
		}

		public float rotate
		{
			get
			{
				return (float)this._rotate / 100f;
			}
			set
			{
				this._rotate = (short)(value * 100f);
			}
		}

		[ProtoMember(4)]
		public long tick
		{
			get;
			set;
		}
	}
}
