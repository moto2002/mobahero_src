using System;

namespace MobaProtocol
{
	public enum MessageType : byte
	{
		RequestFriend = 1,
		SendChip,
		BuyChips = 4,
		AppScore
	}
}
