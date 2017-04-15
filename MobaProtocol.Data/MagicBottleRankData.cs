using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MagicBottleRankData
	{
		[ProtoMember(1)]
		public int rank
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string name
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long todayexp
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int icon
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int level
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int magicbottlelevel
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int pictureFrame
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int CharmRankValue
		{
			get;
			set;
		}
	}
}
