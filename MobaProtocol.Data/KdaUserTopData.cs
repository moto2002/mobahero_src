using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KdaUserTopData
	{
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(1)]
		public int mostWin
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int mostKill
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int mostMoney
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int mostKillMonster
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int mostKeepKill
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int mostAssist
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int mostAllDamage
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int mostAllBeDamaged
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int mostKda
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int totalMvp
		{
			get;
			set;
		}

		public int mostWinNew
		{
			get;
			set;
		}
	}
}
