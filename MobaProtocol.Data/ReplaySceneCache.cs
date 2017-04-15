using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ReplaySceneCache
	{
		[ProtoMember(1)]
		public long time
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long frame
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public byte[] infightInfo
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<RelpayCmd> cmds
		{
			get;
			set;
		}
	}
}
