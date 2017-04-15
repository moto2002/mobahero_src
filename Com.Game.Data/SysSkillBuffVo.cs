using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysSkillBuffVo
	{
		public string unikey;

		public string buff_id;

		public int NewBuff_id;

		public int NewBuff_Level;

		public string buff_name;

		public string buff_type;

		public float buff_time;

		public float buff_time_upgreade;

		public float effective_time;

		public string damage_id;

		public int revert;

		public int max_layers;

		public int clear_flag;

		public string buff_mutex_id;

		public string perform_id;

		public string attach_higheff;

		public string end_attach_higheff;

		public string attach_buff;

		public string end_attach_buff;

		public float probability;

		public int show_icon;

		public string attach_states;

		public string buff_icon;

		public float isReaptPerform;

		public string superposition;
	}
}
