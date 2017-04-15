using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HitSkillInfo
	{
		[ProtoMember(2)]
		public short _skillId;

		[ProtoMember(4)]
		public short _performId;

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

		[ProtoMember(3)]
		public List<short> targetIds
		{
			get;
			set;
		}

		public string performId
		{
			get
			{
				return Name.getString((int)this._performId);
			}
			set
			{
				this._performId = (short)Name.getId(value);
			}
		}

		[ProtoMember(5)]
		public bool bForceDestroy
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int index
		{
			get;
			set;
		}
	}
}
