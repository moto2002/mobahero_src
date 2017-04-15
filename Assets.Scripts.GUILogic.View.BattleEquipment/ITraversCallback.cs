using Com.Game.Data;
using System;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public interface ITraversCallback
	{
		Func<SysBattleItemsVo, int, bool> TraversingCallback
		{
			get;
		}

		object Result
		{
			get;
		}
	}
}
