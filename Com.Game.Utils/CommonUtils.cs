using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Com.Game.Utils
{
	public static class CommonUtils
	{
		public static float GetLeftSeconds(DateTime? time, float maxSeconds)
		{
			float num = maxSeconds;
			if (time.HasValue)
			{
				num = (float)(DateTime.Now - time.Value).TotalSeconds;
			}
			return maxSeconds - num;
		}

		public static bool IsGold(this RewardModel model)
		{
			return model.Type == 1 && model.Id == "1";
		}

		public static bool IsDiamond(this RewardModel model)
		{
			return model.Type == 1 && model.Id == "2";
		}

		public static void MergeRewards(List<RewardModel> rewards)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < rewards.Count; i++)
			{
				if (rewards[i].IsGold())
				{
					num += rewards[i].Count;
				}
				if (rewards[i].IsDiamond())
				{
					num2 += rewards[i].Count;
				}
			}
			rewards.RemoveAll((RewardModel x) => x.IsGold() || x.IsDiamond());
			rewards.Add(new RewardModel
			{
				Count = num,
				Id = "1",
				Type = 1
			});
			rewards.Add(new RewardModel
			{
				Count = num2,
				Id = "2",
				Type = 1
			});
		}
	}
}
