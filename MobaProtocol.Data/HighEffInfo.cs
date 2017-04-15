using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HighEffInfo
	{
		[ProtoMember(2)]
		public short _highEffId;

		[ProtoMember(6)]
		public short _skillId;

		[ProtoMember(1)]
		public List<short> unitIds
		{
			get;
			set;
		}

		public string highEffId
		{
			get
			{
				return Name.getString((int)this._highEffId);
			}
			set
			{
				this._highEffId = (short)Name.getId(value);
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
		public SVector3 skillPosition
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public short _ownerUnitId
		{
			get;
			set;
		}

		public int ownerUnitId
		{
			get
			{
				return (int)this._ownerUnitId;
			}
			set
			{
				this._ownerUnitId = (short)value;
			}
		}

		public string skillId
		{
			get
			{
				return Name.getString((int)this._skillId);
			}
			set
			{
				this._skillId = (short)Name.getId(value);
			}
		}

		[ProtoMember(7)]
		public float rotatoY
		{
			get;
			set;
		}
	}
}
