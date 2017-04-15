using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using System;
using System.Collections.Generic;

namespace Com.Game.Module
{
	public static class RewardTools
	{
		public static List<RewardItemBase> GetRewardInfo(string rewardID)
		{
			List<RewardItemBase> list = new List<RewardItemBase>();
			SysDropRewardsVo dataById = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(rewardID);
			if (dataById == null)
			{
				ClientLogger.Error("配置错误，SysDropRewardsVo找不到 id=" + rewardID);
			}
			else
			{
				string[] array = dataById.drop_items.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					SysDropItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(array[i]);
					if (dataById2 != null)
					{
						RewardItemBase rewardItem = RewardTools.GetRewardItem(dataById2.rewards);
						if (rewardItem != null)
						{
							list.Add(rewardItem);
						}
					}
				}
			}
			return list;
		}

		private static RewardItemBase GetRewardItem(string typeStr)
		{
			RewardItemBase rewardItemBase = null;
			if (string.IsNullOrEmpty(typeStr))
			{
				return rewardItemBase;
			}
			string[] array = typeStr.Split(new char[]
			{
				'|'
			});
			if (array.Length != 3)
			{
				ClientLogger.Error("配置错误");
				return rewardItemBase;
			}
			int num = 0;
			if (!int.TryParse(array[0], out num))
			{
				return rewardItemBase;
			}
			int num2 = 0;
			if (!int.TryParse(array[1], out num2))
			{
				return rewardItemBase;
			}
			switch (num)
			{
			case 1:
				rewardItemBase = new Reward_currency();
				break;
			case 2:
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(array[1]);
				if (dataById != null)
				{
					if (dataById.type == 4)
					{
						rewardItemBase = new Reward_runes();
					}
					else if (dataById.type == 10 && dataById.hero_decorate_type >= 1 && dataById.hero_decorate_type <= 7)
					{
						rewardItemBase = new Reward_effect();
					}
					else
					{
						rewardItemBase = new Reward_item();
					}
				}
				break;
			}
			case 3:
				switch (num2)
				{
				case 1:
					rewardItemBase = new Reward_hero(false);
					break;
				case 2:
					rewardItemBase = new Reward_skin(false);
					break;
				case 3:
					rewardItemBase = new Reward_summonerHeadPortrait();
					break;
				case 4:
					rewardItemBase = new Reward_summonerFrame();
					break;
				case 5:
				{
					SysCouponVo dataById2 = BaseDataMgr.instance.GetDataById<SysCouponVo>(array[2]);
					if (dataById2 != null)
					{
						if (dataById2.mother_type == 1)
						{
							rewardItemBase = new Reward_hero(true);
						}
						else if (dataById2.mother_type == 2)
						{
							rewardItemBase = new Reward_skin(true);
						}
					}
					break;
				}
				}
				break;
			case 4:
			{
				int num3 = num2;
				if (num3 != 1)
				{
					if (num3 == 2)
					{
						rewardItemBase = new Reward_bottleExp();
					}
				}
				else
				{
					rewardItemBase = new Reward_summonerExp();
				}
				break;
			}
			case 6:
				rewardItemBase = new Reward_gameBuff();
				break;
			}
			if (rewardItemBase == null)
			{
				ClientLogger.Error(string.Concat(new string[]
				{
					"配置错误,找不到对应奖励类型:param=",
					array[0],
					",",
					array[1],
					",",
					array[2]
				}));
			}
			else
			{
				rewardItemBase.Init(array);
			}
			return rewardItemBase;
		}
	}
}
