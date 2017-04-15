using System;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public interface ITraversListCallback
	{
		Func<object, int, bool> TraversingCallback
		{
			get;
		}

		object Result
		{
			get;
		}
	}
}
