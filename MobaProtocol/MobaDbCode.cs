using System;

namespace MobaProtocol
{
	public enum MobaDbCode : byte
	{
		GeneralTransmitCode = 1,
		Notification = 10,
		Distributor,
		FaitourLogin,
		PayDimond,
		PayResult,
		CloseServer,
		NofityGameServer,
		MagicBottleRank,
		GetMagicBottleRank,
		NotifyGameUser,
		GetSummonerLadderRank,
		DealBattleEnd = 27,
		GetCharmRank,
		RealCloseServer,
		PaiWeiError,
		AddGameLog,
		CloseUserAccount = 33,
		LoadUserFightRecord,
		DealUserGameReport,
		DealRichManGift
	}
}
