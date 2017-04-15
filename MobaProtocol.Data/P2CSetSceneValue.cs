using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CSetSceneValue
	{
		[ProtoMember(1)]
		public string key
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string val
		{
			get;
			set;
		}
	}
}
