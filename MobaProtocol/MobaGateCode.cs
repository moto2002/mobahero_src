using System;

namespace MobaProtocol
{
	public enum MobaGateCode : byte
	{
		OnlineCount = 10,
		SelectGameServer = 20,
		VerificationKey,
		RegisterKey = 30,
		DisRegisterKey,
		RegisterGate,
		RegisterSession,
		KickOut = 40,
		GetLobbyBack
	}
}
