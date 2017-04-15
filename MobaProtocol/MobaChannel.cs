using System;

namespace MobaProtocol
{
	public enum MobaChannel : byte
	{
		Default,
		Game,
		Lobby,
		PVP,
		ServiceTypeBegin = 10,
		Session,
		Friend,
		Chat,
		Team,
		Union,
		UserData,
		PlayerState,
		ServiceTypeEnd = 20
	}
}
