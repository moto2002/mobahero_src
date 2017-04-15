using System;

namespace MobaProtocol.Data
{
	public enum SurrenderErrorCode : byte
	{
		OK,
		StartLimit,
		StartShortGameTime,
		StartCD,
		StartInProgress,
		VoteAlready,
		VoteFinish,
		VoteInvalidVoter
	}
}
