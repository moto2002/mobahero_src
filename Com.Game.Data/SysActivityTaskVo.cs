using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysActivityTaskVo
	{
		public string unikey;

		public int id;

		public string describe;

		public int task_type;

		public int task;

		public string condition;

		public int parameter;

		public string reward;

		public int travel_to;
	}
}
