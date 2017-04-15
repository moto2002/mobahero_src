using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NoticeBoardData
	{
		[ProtoMember(1)]
		public int noticeid
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
		public string titleBtn
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string bodytext
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int label
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int type
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string starttime
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public string endtime
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public bool hasRead
		{
			get;
			set;
		}
	}
}
