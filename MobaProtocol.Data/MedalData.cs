using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MedalData
	{
		[ProtoMember(1)]
		public bool Brave
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public bool Death
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public bool Raider
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public bool Solitaire
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public bool King
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public bool Team
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public bool Money
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public bool Diamond
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public bool Summoner
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public bool Season
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public bool Teammaster
		{
			get;
			set;
		}
	}
}
