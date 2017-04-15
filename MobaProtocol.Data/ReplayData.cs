using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ReplayData
	{
		[ProtoMember(1)]
		public ReplayHeader header
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<RelpayCmd> cmds
		{
			get;
			set;
		}
	}
}
