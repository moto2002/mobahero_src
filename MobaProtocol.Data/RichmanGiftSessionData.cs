using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RichmanGiftSessionData
	{
		[ProtoMember(1)]
		public int diamond
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string OrderId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int count
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<int> itemlist
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string ChargeUid
		{
			get;
			set;
		}
	}
}
