using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_coupon : RewardItemBase
	{
		public Reward_coupon() : base(ERewardType.Reward_coupon)
		{
		}

		public override void Init(string[] param)
		{
			base.Init(param);
			if (base.Valid)
			{
				base.Num = 1;
				SysCouponVo dataById = BaseDataMgr.instance.GetDataById<SysCouponVo>(param[2]);
				if (dataById != null)
				{
					if (dataById.mother_type == 1)
					{
						SysHeroMainVo dataById2 = BaseDataMgr.instance.GetDataById<SysHeroMainVo>(dataById.mother_id);
						if (dataById2 != null)
						{
							base.SIcon = dataById2.avatar_icon;
							base.BIcon = dataById2.Loading_icon;
						}
					}
					else if (dataById.mother_type == 2)
					{
						SysHeroSkinVo dataById3 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(dataById.mother_id);
						if (dataById3 != null)
						{
							base.SIcon = dataById3.avatar_icon;
							base.BIcon = dataById3.Loading_icon;
						}
					}
					base.ExtraData = dataById;
					base.TypeDes = LanguageManager.Instance.GetStringById("Currency_Coupon");
					base.Quality = dataById.quality;
					base.Valid = true;
				}
			}
		}
	}
}
