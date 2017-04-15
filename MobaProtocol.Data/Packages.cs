using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class Packages
	{
		[ProtoMember(1)]
		public long frameId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long tick
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long svrTime
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public RelpayCmd[] packages
		{
			get;
			set;
		}
	}
}
