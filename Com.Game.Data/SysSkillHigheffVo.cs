using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysSkillHigheffVo
	{
		public string unikey;

		public string higheff_id;

		public int NewHigheff_ID;

		public int NewHigheff_Level;

		public string higheff_name;

		public int higheff_trigger;

		public string higheff_condition;

		public string higheff_type;

		public string target_type;

		public string effective_range;

		public string perform_id;

		public string damage_id;

		public string attach_higheff;

		public string attach_buff;

		public string attach_self_higheff;

		public string cd_time;

		public float probability;

		public string effectGain;

		public int interrupt;

		public int isAutoDestroy;
	}
}
