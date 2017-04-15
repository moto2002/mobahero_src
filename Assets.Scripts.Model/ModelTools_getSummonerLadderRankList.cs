using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_getSummonerLadderRankList
	{
		public static SummonerLadderData Get_GetSummonerLadderRankList_X(this ModelManager mmng)
		{
			return mmng.GetSummonerLadderRankList();
		}

		private static SummonerLadderData GetSummonerLadderRankList(this ModelManager mmng)
		{
			SummonerLadderData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_getSummonerLadderRankList))
			{
				SummonerLadderData data = mmng.GetData<SummonerLadderData>(EModelType.Model_getSummonerLadderRankList);
				if (data != null)
				{
					result = data;
				}
			}
			return result;
		}
	}
}
