using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class LuckyMoneystatus
	{
		[ProtoMember(1)]
		public long resttime
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int status
		{
			get;
			set;
		}
	}
}
