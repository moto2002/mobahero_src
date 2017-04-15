using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CGMCheat
	{
		[ProtoMember(1)]
		public string cheatMsg
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int speed
		{
			get;
			set;
		}
	}
}
