using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class EnemyInfoModel
	{
		[ProtoMember(1)]
		public long SummonerId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long HeroId
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
		public int Grade
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Ep_1
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int Ep_2
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int Ep_3
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int Ep_4
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int Ep_5
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int Ep_6
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public int Solestones
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public string ModelId
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public long Exp
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public List<SkillModel> SkillList
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public int LostLife
		{
			get;
			set;
		}

		[ProtoMember(16)]
		public int MP
		{
			get;
			set;
		}

		[ProtoMember(17)]
		public int CurrSkin
		{
			get;
			set;
		}
	}
}
