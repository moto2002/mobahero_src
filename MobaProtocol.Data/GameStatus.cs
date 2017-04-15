using System;

namespace MobaProtocol.Data
{
	public enum GameStatus : sbyte
	{
		Offline,
		Idle,
		Doing,
		AFK,
		PVE_Fighting,
		PVE_SmallFighting,
		PVE_TBCFighting,
		PVP_InRoom,
		PVP_Queueing = 50,
		PVP_Readying,
		PVP_Fighting,
		PVP_Outline,
		PVP_TeamQueueing,
		PVP_LookFriendFight
	}
}
