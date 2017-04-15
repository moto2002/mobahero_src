using System;

namespace MobaProtocol
{
	public enum MobaGateOpKey : byte
	{
		Channel = 255,
		GameId = 0,
		SessionId,
		LobbyId,
		ConnectionId = 4,
		Token,
		Verification,
		CastType = 11,
		UserId = 57,
		AccountId = 71,
		Data = 100,
		DataEnd = 120
	}
}
