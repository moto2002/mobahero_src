using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysPromptVo
	{
		public string unikey;

		public int prompt_id;

		public int prompt_module;

		public string prompt_text;

		public float text_time;

		public float icon_time;

		public string PromptCondition;

		public string sound;

		public string sound2;
	}
}
