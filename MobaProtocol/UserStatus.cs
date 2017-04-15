using System;

namespace MobaProtocol
{
	public enum UserStatus : byte
	{
		Offline = 1,
		Playing,
		Idle,
		Suspend
	}
}
