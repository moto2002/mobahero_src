using ProtoBuf;
using System;
using System.Diagnostics;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CUpSkillLevel
	{
		[ProtoMember(1)]
		public short _skillId;

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

		[ProtoMember(2)]
		public byte skillLevel
		{
			get;
			set;
		}

		[ProtoMember(3)]
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
	}
}
