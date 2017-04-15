using System;

namespace MobaProtocol
{
	public enum MobaSessionCode : byte
	{
		GS2Se_Register = 1,
		GS2Se_Login,
		Se2GS_UpdateAchieve,
		GS2Se_UpdateHeroCount,
		GS2Se_UpdateTicket,
		GS2Se_UpdateNickname,
		GS2Se_UpdatePlayerStatus,
		GS2Se_UpdateIconFrame,
		GS2Se_UpdateMagicBottle,
		GS2Se_UpdateRandomValue,
		GS2Se_UpdateSummSkills,
		GS3Se_GameNotification,
		GS2Se_GameGetOnlineUser,
		Se2GS_GameSaveRichGift,
		GS2Se_MAX = 255
	}
}
