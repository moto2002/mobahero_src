using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class BuffInfo
	{
		[ProtoMember(2)]
		public short _buffId;

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

		public string buffId
		{
			get
			{
				return Name.getString((int)this._buffId);
			}
			set
			{
				this._buffId = (short)Name.getId(value);
			}
		}

		[ProtoMember(3)]
		public short _casterUnitId
		{
			get;
			set;
		}

		public int casterUnitId
		{
			get
			{
				return (int)this._casterUnitId;
			}
			set
			{
				this._casterUnitId = (short)value;
			}
		}

		[ProtoMember(4)]
		public short reduce_layers
		{
			get;
			set;
		}
	}
}
