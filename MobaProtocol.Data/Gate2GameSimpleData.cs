using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class Gate2GameSimpleData
	{
		[ProtoMember(1)]
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string accountid
		{
			get;
			set;
		}
	}
}
