using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TeamKillInfo
	{
		[ProtoMember(1)]
		public int attackerId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string targetTypeid
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public byte targetGroup
		{
			get;
			set;
		}
	}
}
