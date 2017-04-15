using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PlayerCounter
	{
		[ProtoMember(1)]
		public int killHoreCount
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int killMonsterCount
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int helpKillHoreCount
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int deadCount
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public bool isFirstBlood
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public Dictionary<int, int> extKillCount
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int killTowerCount
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public long allDamage
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int allmoney
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public List<string> equiplist
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public long allBeDamage
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public bool isMostMoney
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public Dictionary<int, int> killMonsterDetail
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public bool isKillCrystal
		{
			get;
			set;
		}
	}
}
