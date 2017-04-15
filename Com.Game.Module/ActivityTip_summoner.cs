using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class ActivityTip_summoner : ActivityTip_common
	{
		public UITexture tex_card;

		public override void RefrehUI()
		{
			this.tex_card.mainTexture = ResourceManager.Load<Texture>(base.Info.BIcon, true, true, null, 0, false);
		}

		public override bool IsThisCom(ERewardType t)
		{
			return t == ERewardType.Reward_summonerFrame || ERewardType.Reward_summonerHeadPortrait == t;
		}
	}
}
