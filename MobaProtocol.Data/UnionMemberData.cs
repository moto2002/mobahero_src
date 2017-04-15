using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UnionMemberData
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long Exp
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string Name
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string Icon
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Job
		{
			get;
			set;
		}
	}
}
