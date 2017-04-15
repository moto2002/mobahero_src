using System;

namespace Com.Game.Module
{
	public class BaseControl<T> : Singleton<T> where T : new()
	{
		protected BaseControl()
		{
			this.NetListener();
		}

		protected virtual void NetListener()
		{
		}
	}
}
