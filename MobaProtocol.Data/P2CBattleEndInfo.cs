using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CBattleEndInfo
	{
		[ProtoMember(1)]
		public byte winGroup
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string sceneId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public PvpTeamInfo teamInfo
		{
			get;
			set;
		}
	}
}
