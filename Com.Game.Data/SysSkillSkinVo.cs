using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysSkillSkinVo
	{
		public string unikey;

		public string skill_id;

		public string skill_name;

		public string skill_description;

		public string skill_description2;

		public string skill_description3;

		public string skill_icon;

		public int skill_index;

		public int skill_type;

		public int skill_logic_type;

		public int skill_trigger;

		public int skill_prop;

		public float distance;

		public string target_type;

		public string effective_range;

		public int max_num;

		public string need_target;

		public float sing_time;

		public string guide_time;

		public int interrupt;

		public string cost;

		public float cost_upgrade;

		public string skill_phase;

		public float skill_interval;

		public float cd;

		public float cd_upgrade;

		public float hard_cd;

		public string skill_mutex;

		public string ready_action_ids;

		public string start_action_ids;

		public string hit_action_ids;

		public string end_action_ids;

		public string init_higheff_ids;

		public string start_higheff_ids;

		public string hit_higheff_ids;

		public string start_buff_ids;

		public string hit_buff_ids;

		public int hit_trigger_type;

		public string hit_time;

		public string damage_id;
	}
}
