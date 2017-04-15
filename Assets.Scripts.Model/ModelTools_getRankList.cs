using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_getRankList
	{
		public static MyRankList Get_GetRankList_X(this ModelManager mmng)
		{
			return mmng.GetMyRankList();
		}

		private static MyRankList GetMyRankList(this ModelManager mmng)
		{
			MyRankList result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_getRankList))
			{
				MyRankList data = mmng.GetData<MyRankList>(EModelType.Model_getRankList);
				if (data != null)
				{
					result = data;
				}
			}
			return result;
		}
	}
}
