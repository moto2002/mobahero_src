using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class CharmRankData
	{
		[ProtoMember(1)]
		public int Rank
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string NickName
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public long Charm
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int IconId
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
		public int MagicBottleRank
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int CharmRankValue
		{
			get;
			set;
		}
	}
}
