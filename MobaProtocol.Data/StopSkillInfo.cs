using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class StopSkillInfo
	{
		[ProtoMember(2)]
		public short _skillId;

		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
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
		public byte skillStep
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public byte stopSkillType
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public byte interruptType
		{
			get;
			set;
		}
	}
}
