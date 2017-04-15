using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DoubleCardData
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
		public int type
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public DateTime timeget
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int recordvalue
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int recordtype
		{
			get;
			set;
		}
	}
}
