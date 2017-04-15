using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysNotificationVo
	{
		public string unikey;

		public int notice_id;

		public string description;

		public string notice_type;

		public string notice_desc;

		public string start_time;

		public string end_time;

		public string push_time;
	}
}
