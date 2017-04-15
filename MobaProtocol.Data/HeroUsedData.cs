using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HeroUsedData
	{
		[ProtoMember(1)]
		public string heroid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int useinfo
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int wincount
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int losecount
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public DateTime updatetime
		{
			get;
			set;
		}
	}
}
