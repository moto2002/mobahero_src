using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RewardModel
	{
		[ProtoMember(1)]
		public int Type
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string Id
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Count
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public long OnlyId
		{
			get;
			set;
		}
	}
}
