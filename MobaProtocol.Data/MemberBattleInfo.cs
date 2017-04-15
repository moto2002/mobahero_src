using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MemberBattleInfo
	{
		[ProtoMember(1)]
		public string userId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string userName
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Level
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int Icon
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int PictureFrame
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int gourpId
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public bool isWin
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public string heroId
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public PlayerCounter playercounter
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public double ladderScore
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public int CharmRankvalue
		{
			get;
			set;
		}
	}
}
