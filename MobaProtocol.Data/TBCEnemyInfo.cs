using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TBCEnemyInfo
	{
		[ProtoMember(1)]
		public bool IsWin
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long BattleId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public bool IsReceive
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string Reward
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public List<EnemyInfoModel> List
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string ServerId
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public long Exp
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public string NickName
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public string UnionName
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int Icon
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public int PictureFrameId
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public string AcountId
		{
			get;
			set;
		}
	}
}
