using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class AccountData
	{
		[ProtoMember(1)]
		public string AccountId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string UserName
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string Password
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string Mail
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int UserType
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int DeviceType
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string DeviceToken
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int ServerName
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public string ChannelId
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public string PlatformUid
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public string Channel
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public string ChannelUid
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public string AccessToken
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int ProductId
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public bool IsBindPhone
		{
			get;
			set;
		}

		[ProtoMember(16)]
		public bool IsBindEmail
		{
			get;
			set;
		}
	}
}
