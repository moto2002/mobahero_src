using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HeroInfo
	{
		[ProtoMember(1)]
		public string heroId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int quality
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int star
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
		public HeroInfoData Hero
		{
			get;
			set;
		}
	}
}
