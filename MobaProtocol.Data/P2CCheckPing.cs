using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CCheckPing
	{
		[ProtoMember(1)]
		public long serverTime
		{
			get;
			set;
		}
	}
}
