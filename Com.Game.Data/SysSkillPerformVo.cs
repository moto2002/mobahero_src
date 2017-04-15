using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysSkillPerformVo
	{
		public string unikey;

		public string perform_id;

		public string perform_name;

		public string action_id;

		public float action_time;

		public float action_delay;

		public string effect_type;

		public float effect_speed;

		public string use_collider;

		public string extra_param;

		public string effect_id;

		public float effect_time;

		public float effect_delay;

		public int effect_pos_type;

		public int effect_anchor;

		public string effect_pos_offset;

		public string effect_rotation_offset;

		public string sound_id;

		public float sound_time;

		public float sound_delay;

		public int sound_volume;

		public int body_dispear;

		public int body_dissolve;

		public int body_destroy;

		public float time_scale;
	}
}
