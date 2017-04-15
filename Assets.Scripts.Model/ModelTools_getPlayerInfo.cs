using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_getPlayerInfo
	{
		public static PlayerData Get_GetPlayerData_X(this ModelManager mmng)
		{
			return mmng.GetPlayerData();
		}

		private static PlayerData GetPlayerData(this ModelManager mmng)
		{
			PlayerData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_getPlayerInfo))
			{
				PlayerData data = mmng.GetData<PlayerData>(EModelType.Model_getPlayerInfo);
				if (data != null)
				{
					result = data;
				}
			}
			return result;
		}
	}
}
