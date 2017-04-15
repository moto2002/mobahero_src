using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysBattleRefreshVo
	{
		public string unikey;

		public string refreshid;

		public string trigger_condition;

		public int delay_time;

		public int refresh_way;

		public string locationid;

		public string monster_mainid;

		public int refresh_number;

		public int cycle_number;

		public int cycle_interval;
	}
}
