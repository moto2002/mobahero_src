using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class AwardInfoData
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
		public bool IsTimeAward
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string BossId
		{
			get;
			set;
		}
	}
}
