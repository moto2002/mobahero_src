using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SummSkinData
	{
		[ProtoMember(1)]
		public long SummId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int SkinId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string NpcId
		{
			get;
			set;
		}
	}
}
