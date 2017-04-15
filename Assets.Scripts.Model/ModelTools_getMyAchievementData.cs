using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_getMyAchievementData
	{
		public static MyAchievementInfo Get_GetMyAchievementData_X(this ModelManager mmng)
		{
			return mmng.GetMyAchievementData();
		}

		private static MyAchievementInfo GetMyAchievementData(this ModelManager mmng)
		{
			MyAchievementInfo result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_getMyAchievementData))
			{
				MyAchievementInfo data = mmng.GetData<MyAchievementInfo>(EModelType.Model_getMyAchievementData);
				if (data != null)
				{
					result = data;
				}
			}
			return result;
		}

		public static int[] GetBattleTypeInfo(this ModelManager mmng, string typeId)
		{
			int[] array = new int[2];
			KDAData kdaData = mmng.Get_GetMyAchievementData_X().kdaData;
			BattleInfoData[] battleinfos = kdaData.battleinfos;
			for (int i = 0; i < battleinfos.Length; i++)
			{
				BattleInfoData battleInfoData = battleinfos[i];
				if (battleInfoData.battleid == typeId)
				{
					array[0] = battleInfoData.wincount;
					array[1] = battleInfoData.losecount + battleInfoData.wincount;
				}
			}
			return array;
		}

		public static HistoryBattleData CheckBattleRecord(this ModelManager mmng, long _LogId)
		{
			int[] array = new int[2];
			Dictionary<long, HistoryBattleData> historyBattleDataDic = mmng.Get_GetMyAchievementData_X().HistoryBattleDataDic;
			if (historyBattleDataDic.ContainsKey(_LogId))
			{
				return historyBattleDataDic[_LogId];
			}
			return null;
		}

		public static HomeKDAData Get_HomeKDAData(this ModelManager mmng)
		{
			MyAchievementInfo myAchievementData = mmng.GetMyAchievementData();
			return (myAchievementData != null) ? myAchievementData.myHomeKDA : null;
		}
	}
}
