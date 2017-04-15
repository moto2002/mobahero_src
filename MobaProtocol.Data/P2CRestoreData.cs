using ProtoBuf;
using System;
using System.Diagnostics;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CRestoreData
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
				Debug.Assert(-32700 < value && value < 32700);
				this._unitId = (short)value;
			}
		}

		[ProtoMember(2)]
		public float hp
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public float mp
		{
			get;
			set;
		}
	}
}
