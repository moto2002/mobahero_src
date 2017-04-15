using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SessionUserData
	{
		[ProtoMember(1)]
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string nickname
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string accountid
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public long exp
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public long summonerid
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int Icon
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int pictureFrame
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int bottlelevel
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int charm
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int herocount
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public int ticket
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public int gamestatus
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public int LadderScore
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int randomvalue
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public string summskillSaved
		{
			get;
			set;
		}

		[ProtoMember(16)]
		public int CharmRankvalue
		{
			get;
			set;
		}

		[ProtoMember(17)]
		public int rankFrame
		{
			get;
			set;
		}
	}
}
