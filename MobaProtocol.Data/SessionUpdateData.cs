using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SessionUpdateData
	{
		[ProtoMember(1)]
		public string userid
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
		public int data
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string strdata
		{
			get;
			set;
		}
	}
}
