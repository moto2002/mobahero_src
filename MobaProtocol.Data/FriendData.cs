using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class FriendData
	{
		[ProtoMember(1)]
		public long SummId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long TargetId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string TargetName
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public long Exp
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Icon
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int PictureFrame
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int LadderScore
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int MathWinNum
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int LadderRank
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public sbyte Status
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public sbyte GameStatus
		{
			get;
			set;
		}

		public List<Messages> Messages
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public int bottlelevel
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public int charm
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int CharmRankValue
		{
			get;
			set;
		}

		public GameStatus GetGameStatus()
		{
			return (GameStatus)this.GameStatus;
		}
	}
}
