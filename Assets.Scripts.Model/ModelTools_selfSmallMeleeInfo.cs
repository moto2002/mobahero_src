using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_selfSmallMeleeInfo
	{
		public static SmallMeleeData Get_selfSmallMeleeInfo_X(this ModelManager mmng)
		{
			return mmng.GetSelfSmallMeleeInfo();
		}

		private static SmallMeleeData GetSelfSmallMeleeInfo(this ModelManager mmng)
		{
			SmallMeleeData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_selfSmallMeleeInfo))
			{
				SmallMeleeData data = mmng.GetData<SmallMeleeData>(EModelType.Model_selfSmallMeleeInfo);
				if (data != null)
				{
					result = data;
				}
			}
			return result;
		}
	}
}
