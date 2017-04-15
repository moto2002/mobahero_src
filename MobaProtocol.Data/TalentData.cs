using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TalentData
	{
		[ProtoMember(1)]
		public int TalentID
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int TalentCount
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Talent_1
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int Talent_2
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Talent_3
		{
			get;
			set;
		}
	}
}
