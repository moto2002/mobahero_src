using System;

namespace MobaProtocol
{
	public enum MobaShareCode : byte
	{
		ShareStart = 10,
		JoinQueue,
		OutQueue,
		StartGame,
		QueryPvpState,
		RefreshPlayerPvpState,
		ForceQuitPvp,
		LobbyShutdown,
		OnUserQuit,
		PlayerOutline,
		Playerconnect,
		ShareEnd = 50
	}
}
