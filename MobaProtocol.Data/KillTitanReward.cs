using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KillTitanReward
	{
		[ProtoMember(1)]
		public long DropTime
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<RewardModel> RewardData
		{
			get;
			set;
		}

		public DateTime dt
		{
			get
			{
				return new DateTime(this.DropTime);
			}
		}
	}
}
