using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MailData
	{
		[ProtoMember(5)]
		public List<RewardModel> Mail_AwardList;

		[ProtoMember(1)]
		public long Id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int TemplateId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string Mail_Param
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public DateTime Time
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public bool IsRead
		{
			get;
			set;
		}
	}
}
