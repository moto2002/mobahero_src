using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KillTitanModel
	{
		[ProtoMember(1)]
		public string ModelId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long DieTime
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Location
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int CoinReward
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int DimondReward
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int CurrLife
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int MaxLife
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int Act
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public long JoinTime
		{
			get;
			set;
		}

		public DateTime dt
		{
			get
			{
				return new DateTime(this.DieTime);
			}
		}
	}
}
