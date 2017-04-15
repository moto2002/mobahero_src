using System;

namespace MobaClient.MemoryDB
{
	[Serializable]
	public enum MobaRowState
	{
		Insert,
		Delete,
		Update,
		Noting
	}
}
