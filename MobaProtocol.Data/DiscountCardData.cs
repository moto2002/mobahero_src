using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DiscountCardData
	{
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(1)]
		public int modelid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public DateTime endtime
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string typemodelid
		{
			get;
			set;
		}
	}
}
