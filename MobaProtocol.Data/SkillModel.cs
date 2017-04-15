using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SkillModel
	{
		[ProtoMember(1)]
		public string SkillId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Level
		{
			get;
			set;
		}
	}
}
