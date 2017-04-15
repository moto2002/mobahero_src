using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SweepData
	{
		[ProtoMember(1)]
		public List<RewardModel> Reward
		{
			get;
			set;
		}
	}
}
