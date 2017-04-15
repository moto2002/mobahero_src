using System;
using System.Collections.Generic;

namespace Com.Game.Module
{
	public class RecyclerManager
	{
		public static List<Action> _onExitList = new List<Action>();

		public static void AddOnExit(Action onExit)
		{
			RecyclerManager._onExitList.Add(onExit);
		}

		public static void Exit()
		{
			RecyclerManager._onExitList.ForEach(delegate(Action x)
			{
				x();
			});
			RecyclerManager._onExitList.Clear();
		}
	}
}
