using System;

namespace MobaHeros.AI
{
	public class WatchTarget
	{
		public float CreateTime
		{
			get;
			protected set;
		}

		public Units Target
		{
			get;
			protected set;
		}

		public WatchTarget(float curTime, Units target)
		{
			this.CreateTime = curTime;
			this.Target = target;
		}
	}
}
