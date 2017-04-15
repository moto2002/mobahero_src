using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ArenaState
	{
		[ProtoMember(1)]
		public string ServerTime
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int LaveTick
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int LaveCount
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int DayRestArenaCount
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int DayRestCDTimeCount
		{
			get;
			set;
		}
	}
}
