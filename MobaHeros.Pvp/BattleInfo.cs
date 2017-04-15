using MobaProtocol.Data;
using System;

namespace MobaHeros.Pvp
{
	public class BattleInfo
	{
		public int BattleId;

		public PvpJoinType JoinType;

		public BattleInfo()
		{
			this.JoinType = PvpJoinType.Invalid;
		}
	}
}
