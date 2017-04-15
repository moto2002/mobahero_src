using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_summonerFrame : RewardItemBase
	{
		public Reward_summonerFrame() : base(ERewardType.Reward_summonerFrame)
		{
		}

		public override void Init(string[] param)
		{
			base.Init(param);
			if (base.Valid)
			{
				base.Num = 1;
				SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(param[2]);
				if (dataById != null)
				{
					base.SIcon = dataById.pictureframe_icon;
					base.BIcon = dataById.pictureframe_icon;
					base.TypeDes = LanguageManager.Instance.GetStringById("Currency_PictureFrame");
					base.Des = LanguageManager.Instance.GetStringById(dataById.pictureframe_name);
					base.Quality = dataById.pictureframe_quality;
					base.Valid = true;
				}
			}
		}
	}
}
