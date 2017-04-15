using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MailNoticeData
	{
		[ProtoMember(1)]
		public string id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string title
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string sender
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string time
		{
			get;
			set;
		}
	}
}
