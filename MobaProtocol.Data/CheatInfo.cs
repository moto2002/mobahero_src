using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class CheatInfo
	{
		[ProtoMember(1)]
		public string cheatMsg
		{
			get;
			set;
		}
	}
}
