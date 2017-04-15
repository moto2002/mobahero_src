using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PlayerChangeHeroInfo
	{
		[ProtoMember(1)]
		public int newUid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<int> canChangeNewUids
		{
			get;
			set;
		}
	}
}
