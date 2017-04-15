using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ArenaData
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long Rank
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Icon
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int PictureFrameId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Exp
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int WinCount
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string NickName
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public List<HeroInfoData> List
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int Power
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public long LastDayRank
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public int AtkWinCount
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public int DefWinCount
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public int RankUp
		{
			get;
			set;
		}
	}
}
