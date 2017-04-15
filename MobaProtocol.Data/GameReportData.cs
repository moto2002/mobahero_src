using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class GameReportData
	{
		[ProtoMember(1)]
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public ReportType reportype
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int battleid
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string roomgid
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public byte allytype
		{
			get;
			set;
		}

		public string reporter
		{
			get;
			set;
		}

		public DateTime timereport
		{
			get;
			set;
		}

		public long id
		{
			get;
			set;
		}
	}
}
