using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Game.Module
{
	public class Reward_hero : RewardItemBase
	{
		public Reward_hero(bool discount = false) : base(ERewardType.Reward_hero)
		{
			base.Discount = discount;
		}

		public override void Init(string[] param)
		{
			base.Init(param);
			if (base.Valid)
			{
				base.Num = 1;
				SysHeroMainVo sysHeroMainVo = null;
				if (base.Discount)
				{
					SysCouponVo dataById = BaseDataMgr.instance.GetDataById<SysCouponVo>(param[2]);
					base.ExtraData = dataById;
					sysHeroMainVo = BaseDataMgr.instance.GetHeroMainData(dataById.mother_id);
				}
				else
				{
					int heroId = 0;
					if (int.TryParse(param[2], out heroId))
					{
						Dictionary<string, SysHeroMainVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroMainVo>();
						sysHeroMainVo = typeDicByType.Values.FirstOrDefault((SysHeroMainVo obj) => obj.hero_id == heroId);
					}
				}
				if (sysHeroMainVo != null)
				{
					base.SIcon = sysHeroMainVo.avatar_icon;
					base.BIcon = sysHeroMainVo.Loading_icon;
					if (base.Discount)
					{
						base.TypeDes = LanguageManager.Instance.GetStringById("Currency_Coupon");
					}
					else
					{
						base.TypeDes = LanguageManager.Instance.GetStringById("Currency_Hero");
						base.Des = LanguageManager.Instance.GetStringById(sysHeroMainVo.name);
					}
					base.Quality = sysHeroMainVo.quality;
					base.Valid = true;
				}
			}
		}
	}
}
