using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UnionInfoData
	{
		[ProtoMember(1)]
		public int Id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string Name
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Type
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int IconId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int IconborderId
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int JoinLevel
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string Announcement
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int Level
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int MemberCount
		{
			get;
			set;
		}
	}
}
