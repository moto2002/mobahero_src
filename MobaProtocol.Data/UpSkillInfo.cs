using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UpSkillInfo
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
	}
}
