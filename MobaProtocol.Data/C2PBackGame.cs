using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class C2PBackGame
	{
		[ProtoMember(1)]
		public int roomId
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
		public string newkey
		{
			get;
			set;
		}
	}
}
