using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PvpTeamMember
	{
		[ProtoMember(1)]
		public PvpMemberInfo baseInfo
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public HeroInfo heroInfo
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public byte group
		{
			get;
			set;
		}

		public string Uid
		{
			get
			{
				return this.baseInfo.userId;
			}
		}
	}
}
