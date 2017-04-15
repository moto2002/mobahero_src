using Com.Game.Module;
using System;
using System.Collections.Generic;

namespace MobaHeros.Pvp
{
	internal struct LevelStorage
	{
		public MatchType matchType;

		public string battleId;

		public string roomOwnerSummId;

		public List<string> memberSummIds;
	}
}
