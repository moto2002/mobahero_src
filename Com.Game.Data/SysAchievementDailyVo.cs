using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysAchievementDailyVo
	{
		public string unikey;

		public int daily_id;

		public int type;

		public string name;

		public string describe;

		public string icon;

		public int summoners_level;

		public string start_time;

		public string end_time;

		public int task;

		public string condition;

		public int parameter;

		public string reward;

		public int travel_to;

		public string tips;
	}
}
