using System;

namespace MobaProtocol
{
	public enum MobaTeamRoomCode : byte
	{
		Room_Join = 1,
		Room_Levea,
		Room_StartGame,
		Room_ChangeTeamType,
		Room_Destory,
		Room_Create,
		Room_Kick,
		Room_CurrData,
		Room_ChangeRoomType,
		Room_ComeBack,
		Room_ExchangeTeamType,
		Room_OwnerQuit,
		Room_PlayerEscapeCD,
		Room_InviteJoinRoom,
		Room_NotifyErrorInfo,
		Room_RefuseJoinInvite,
		Room_QueryRoomState,
		Room_ChangeLobby = 254,
		Room_Max
	}
}
