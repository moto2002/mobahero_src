using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class LuckyDrawResult
	{
		[ProtoMember(1)]
		public int RewardType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string RewardId
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
		public byte DrawType
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string ModelId
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int SoulNum
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int Grade
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int Star
		{
			get;
			set;
		}
	}
}
