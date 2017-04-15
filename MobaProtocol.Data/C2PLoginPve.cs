using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class C2PLoginPve
	{
		[ProtoMember(1)]
		public int battleId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int roomId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public List<PvePlayerInfo> pvePlayer
		{
			get;
			set;
		}
	}
}
