using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_taskInfoList
	{
		public static AchieveAll Get_AchieveAll_X(this ModelManager mmng)
		{
			AchieveAll result = new AchieveAll();
			if (mmng != null && mmng.ValidData(EModelType.Model_taskInfoList))
			{
				result = mmng.GetData<AchieveAll>(EModelType.Model_taskInfoList);
			}
			return result;
		}
	}
}
