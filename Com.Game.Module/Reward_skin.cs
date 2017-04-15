using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_skin : RewardItemBase
	{
		public Reward_skin(bool discount = false) : base(ERewardType.Reward_skin)
		{
			base.Discount = discount;
		}

		public override void Init(string[] param)
		{
			base.Init(param);
			if (base.Valid)
			{
				base.Num = 1;
				string unikey = string.Empty;
				if (base.Discount)
				{
					SysCouponVo dataById = BaseDataMgr.instance.GetDataById<SysCouponVo>(param[2]);
					base.ExtraData = dataById;
					unikey = dataById.mother_id;
				}
				else
				{
					unikey = param[2];
				}
				SysHeroSkinVo dataById2 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(unikey);
				if (dataById2 != null)
				{
					base.SIcon = dataById2.avatar_icon;
					base.BIcon = dataById2.Loading_icon;
					if (base.Discount)
					{
						base.TypeDes = LanguageManager.Instance.GetStringById("Currency_Coupon");
						base.ExtraData = BaseDataMgr.instance.GetDataById<SysCouponVo>(param[2]);
					}
					else
					{
						base.TypeDes = LanguageManager.Instance.GetStringById("Currency_Skin");
						base.Des = LanguageManager.Instance.GetStringById(dataById2.name);
					}
					base.Quality = dataById2.quality;
					base.Valid = true;
				}
			}
		}
	}
}
