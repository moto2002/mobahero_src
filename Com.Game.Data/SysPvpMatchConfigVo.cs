using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysPvpMatchConfigVo
	{
		public string unikey;

		public int scene_id;

		public string name;

		public int match_score_type;

		public string effect_score_type;

		public string search_parameters;

		public string search_parameters_low;

		public int low_limit;

		public string gang_up_score;

		public string weights;
	}
}
