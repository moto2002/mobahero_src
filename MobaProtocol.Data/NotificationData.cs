using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NotificationData
	{
		public enum EType : short
		{
			updateForbidProtol = -7,
			updateShop,
			publicNotice,
			faitourLogin,
			kickout,
			popContentToClient,
			reConnectRefuse,
			reConnectAllow,
			unionKick,
			unionCanJoin,
			unionReject,
			unionJobChange,
			mysteryShop,
			blackDealer,
			friendApplyOk,
			friendApplyReject,
			privateMsg,
			friendApply,
			gameInvite,
			gameInviteReject,
			gameInviteAccept,
			blackList,
			friendDel,
			summonUpgrade,
			arenaDef,
			selfCurrState,
			newMail,
			summonerSkillUnlock,
			roomIsEmpty,
			roomIsFull,
			roomAlreadyStartGame,
			SetGray,
			AlreadyInRoom,
			TeamRoomClosed,
			TeamRoomFull,
			TeamRoomNotExist,
			TeamRoomNotOwner,
			TeamRoomCanNotChangeRoomType,
			TeamRoomNotIn
		}

		public NotificationData.EType notiType;

		[ProtoMember(1)]
		public short Type
		{
			get
			{
				return this.toShortType(this.notiType);
			}
			set
			{
				this.notiType = this.toEnumType(value);
			}
		}

		[ProtoMember(2)]
		public bool IsSendEmail
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string Content
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int timeVal
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int iparam
		{
			get;
			set;
		}

		private NotificationData.EType toEnumType(short val)
		{
			return (NotificationData.EType)val;
		}

		private short toShortType(NotificationData.EType enumType)
		{
			return (short)enumType;
		}
	}
}
