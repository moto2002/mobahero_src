using System;

namespace MobaProtocol.Data
{
	public enum PvpErrorCode : byte
	{
		OK,
		UnknowError,
		StateError,
		NoRoom,
		WrongPwd,
		UserError,
		NoServer,
		NoUser,
		LobbyColsed,
		InPunish,
		HaveNoHero,
		IsForbidden
	}
}
