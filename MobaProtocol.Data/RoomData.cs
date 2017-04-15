using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RoomData
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string AccountId
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
		public int Icon
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int PictureFrame
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public long Exp
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string RoomId
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public byte TeamType
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public bool IsHomeMain
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int RoomType
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public byte PlayGameType
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public string MapId
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public long SummerId
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public string serverkey
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public int CharmRankValue
		{
			get;
			set;
		}

		public FriendGameType GetPlayGameType()
		{
			return (FriendGameType)this.PlayGameType;
		}
	}
}
