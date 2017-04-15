using System;

namespace MobaProtocol
{
	public enum AwardType : byte
	{
		Guest = 1,
		Normal,
		Pay,
		GuestNone = 10,
		NormalNone = 20,
		PayNone = 30
	}
}
