using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class CreateHeroInfo
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string typeId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int level
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public byte group
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string userName
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string summonerName
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public SVector3 position
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int mainHeroUid
		{
			get;
			set;
		}
	}
}
