using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class LuckyDrawData
	{
		[ProtoMember(1)]
		public int DayCoinGetCount
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int DayFreeCoinGetLaveTick
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int DayFreeDimondGetLaveTick
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string ServerTime
		{
			get;
			set;
		}
	}
}
