using System;

namespace MobaProtocol
{
	public enum TypeState
	{
		Init,
		WaitForPlayers,
		WaitForBlinds,
		Playing,
		Showdown,
		DecideWinners,
		DistributeMoney,
		End
	}
}
