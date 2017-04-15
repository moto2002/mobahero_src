using System;
using System.Collections;

namespace Com.Game.Module
{
	public abstract class PopViewTask
	{
		public abstract IEnumerator Run(PopViewParam param, NewPopView view);
	}
}
