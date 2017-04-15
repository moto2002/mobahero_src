using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ReplayHeader
	{
		[ProtoMember(1)]
		public string battleId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long gameTime
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public byte winGroup
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public ReadyPlayerSampleInfo[] playerInfos
		{
			get;
			set;
		}
	}
}
