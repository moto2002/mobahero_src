using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KdaMyHeroData
	{
		[ProtoMember(1)]
		public HeroUsedData herouseddata
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<int> skinlist
		{
			get;
			set;
		}
	}
}
