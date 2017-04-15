using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	public class ReadyPlayerPullData
	{
		[ProtoMember(1)]
		private List<HeroInfo> heros;
	}
}
