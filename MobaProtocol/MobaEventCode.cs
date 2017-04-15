using System;

namespace MobaProtocol
{
	public enum MobaEventCode
	{
		RequestFriend = 1,
		AddFriend,
		DeleteFriend,
		InviteFriend,
		AcceptFriend,
		BroadcastMessage,
		BroadcastMessageInTable,
		Sit,
		PlayerTurnEnded,
		BetTurnStarted,
		BetTurnEnded,
		GameStarted,
		PlayerHoleCardsChanged,
		GameEnded,
		PlayerWonPot,
		PlayerMoneyChanged,
		TableClosed,
		PlayerTurnBegan,
		PlayerJoined,
		PlayerLeaved,
		SameAccountLogin,
		SendChip,
		ExperienceAdded,
		SendGift,
		Achievement,
		RoomTypeChanged,
		PlayerWonPotImprove,
		PlayersShowCards,
		BroadcastStatusTipsMsg,
		TakenMoneyChanged,
		PlayRankChanegd,
		PropertiesChanged,
		Leave,
		Join,
		LobbyBroadcast,
		RoomBroadcastActorAction,
		RoomBroadcastActorQuit,
		RoomBroadcastActorSpeak
	}
}
