using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_battleList
	{
		public static List<BattlesModel> GetBattleList(this ModelManager mmng)
		{
			List<BattlesModel> result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_battleList))
			{
				result = mmng.GetData<List<BattlesModel>>(EModelType.Model_battleList);
			}
			return result;
		}

		public static List<BattlesModel> Get_battleList_X(this ModelManager mmng)
		{
			return mmng.GetBattleList();
		}
	}
}
