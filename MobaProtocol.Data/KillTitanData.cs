using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KillTitanData
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int OpenLocationNum
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int NextOpenLocationSecond
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<KillTitanModel> KillTitanHerosList
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public List<KillTitanModel> KillTitanBoosList
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public List<KillTitanReward> KillTitanRewardList
		{
			get;
			set;
		}
	}
}
