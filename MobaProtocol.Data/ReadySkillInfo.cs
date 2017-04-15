using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ReadySkillInfo
	{
		[ProtoMember(2)]
		public short _skillId;

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
		public SVector3 targetPosition
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<short> targetUnits
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public PvpUseSkillErrorCode errorCode
		{
			get;
			set;
		}
	}
}
