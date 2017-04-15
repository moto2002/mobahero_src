using System;

namespace MobaProtocol
{
	public enum ServiceType : byte
	{
		Default,
		ServiceTypeBegin = 10,
		Session,
		Friend,
		Chat,
		Team,
		Union,
		UserData,
		PlayerState,
		ServiceTypeEnd = 20,
		ServerPeerService
	}
}
