using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class Gate2SessionSimpleData
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

		[ProtoMember(3)]
		public string nickname
		{
			get;
			set;
		}
	}
}
