using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HeroInfoData
	{
		[ProtoMember(1)]
		public long __SummonerId
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
		public string EpMagic
		{
			get;
			set;
		}

		[ProtoMember(16)]
		public int CurrSkin
		{
			get;
			set;
		}

		[ProtoMember(17)]
		public string Skins
		{
			get;
			set;
		}

		[ProtoMember(18)]
		public int BonusPoint
		{
			get;
			set;
		}

		[ProtoMember(19)]
		public int petId
		{
			get;
			set;
		}

		[ProtoMember(20)]
		public int tailEffectId
		{
			get;
			set;
		}

		[ProtoMember(21)]
		public int levelEffectId
		{
			get;
			set;
		}

		[ProtoMember(22)]
		public int backEffectId
		{
			get;
			set;
		}

		[ProtoMember(23)]
		public int birthEffectId
		{
			get;
			set;
		}

		[ProtoMember(24)]
		public int deathEffectId
		{
			get;
			set;
		}

		[ProtoMember(25)]
		public int eyeUnitSkinId
		{
			get;
			set;
		}
	}
}
