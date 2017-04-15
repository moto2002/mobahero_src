using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_summonerHeadPortrait : RewardItemBase
	{
		public Reward_summonerHeadPortrait() : base(ERewardType.Reward_summonerHeadPortrait)
		{
		}

		public override void Init(string[] param)
		{
			base.Init(param);
			if (base.Valid)
			{
				base.Num = 1;
				SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(param[2]);
				if (dataById != null)
				{
					base.SIcon = dataById.headportrait_icon;
					base.BIcon = dataById.headportrait_icon;
					base.TypeDes = LanguageManager.Instance.GetStringById("Currency_HeadPortrait");
					base.Des = LanguageManager.Instance.GetStringById(dataById.headportrait_name);
					base.Quality = dataById.headportrait_quality;
					base.Valid = true;
				}
			}
		}
	}
}
