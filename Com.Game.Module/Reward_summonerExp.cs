using System;

namespace Com.Game.Module
{
	public class Reward_summonerExp : RewardItemBase
	{
		public Reward_summonerExp() : base(ERewardType.Reward_summonerExp)
		{
		}

		public override void Init(string[] param)
		{
			base.Init(param);
			if (base.Valid)
			{
				int num;
				if (int.TryParse(param[2], out num))
				{
					base.Num = num;
					base.SIcon = "Checkins_icons_exp_summoner";
					base.TypeDes = LanguageManager.Instance.GetStringById("Currency_SummonerExp");
					base.Des = LanguageManager.Instance.GetStringById("Currency_SummonerExp_Desc");
					base.Quality = 1;
					base.Valid = true;
				}
			}
		}
	}
}
