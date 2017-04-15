using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SmallMeleeLogData
	{
		[ProtoMember(1)]
		public long id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string targetid
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string nick
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int level
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int iconid
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public DateTime time
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int rank
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int honorValue
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public bool isWin
		{
			get;
			set;
		}
	}
}
