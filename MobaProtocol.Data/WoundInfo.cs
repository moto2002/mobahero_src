using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class WoundInfo
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
		public short _attackerId
		{
			get;
			set;
		}

		public int attackerId
		{
			get
			{
				return (int)this._attackerId;
			}
			set
			{
				this._attackerId = (short)value;
			}
		}

		[ProtoMember(3)]
		public List<short> dataKeys
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<float> dataValues
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public List<float> dataAfterValues
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public byte damageExtInfo
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int damageType
		{
			get;
			set;
		}
	}
}
