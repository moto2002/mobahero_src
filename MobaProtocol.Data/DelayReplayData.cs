using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DelayReplayData
	{
		[ProtoMember(1)]
		public long currTime
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long currFrame
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public ReplaySceneCache lastFrameData
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
