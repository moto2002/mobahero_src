using System;

namespace MobaClient
{
	public enum MobaDisconnectedType
	{
		Normal,
		HeartBeatTimeOut,
		TimeoutDisconnect,
		DisconnectByServer,
		DisconnectByServerUserLimit,
		DisconnectByServerLogic,
		Num
	}
}
