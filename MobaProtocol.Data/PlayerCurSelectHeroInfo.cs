using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PlayerCurSelectHeroInfo
	{
		[ProtoMember(1)]
		public string userId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int newUid
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string curSelectHeroModelId
		{
			get;
			set;
		}
	}
}
