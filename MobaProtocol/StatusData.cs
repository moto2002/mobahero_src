using ProtoBuf;
using System;

namespace MobaProtocol
{
	[ProtoContract]
	public class StatusData
	{
		[ProtoMember(1)]
		public string userid
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
