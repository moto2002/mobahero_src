using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ReplaySceneCachePacks
	{
		[ProtoMember(1)]
		public List<ReplaySceneCache> packs
		{
			get;
			set;
		}
	}
}
