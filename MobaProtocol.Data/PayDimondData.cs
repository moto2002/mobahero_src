using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PayDimondData
	{
		[ProtoMember(1)]
		public long UserId
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
		public string Channel
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string AppstoreProductId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string Currency
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string CurrencyUnit
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int CurrencyCount
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public DateTime PayDate
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int gameAddCount
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int FirstChargeCount
		{
			get;
			set;
		}

		public long id
		{
			get;
			set;
		}

		public bool issucc
		{
			get;
			set;
		}

		public int shopitemid
		{
			get;
			set;
		}

		public string allinfo
		{
			get;
			set;
		}
	}
}
