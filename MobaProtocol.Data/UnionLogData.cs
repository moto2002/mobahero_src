using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UnionLogData
	{
		[ProtoMember(1)]
		public long Id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int UnionId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int TemplateId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string LogParams
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public DateTime Time
		{
			get;
			set;
		}
	}
}
