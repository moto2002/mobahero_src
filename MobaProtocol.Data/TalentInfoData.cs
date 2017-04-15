using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TalentInfoData
	{
		[ProtoMember(1)]
		public bool IsUse
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int RestCount
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public List<TalentModel> List
		{
			get;
			set;
		}
	}
}
