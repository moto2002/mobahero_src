using System;

namespace Com.Game.Module
{
	public class ActivityTip_rune : ActivityTip_common
	{
		public UISprite sp_icon;

		public override void RefrehUI()
		{
			this.sp_icon.spriteName = base.Info.BIcon;
		}

		public override bool IsThisCom(ERewardType t)
		{
			return ERewardType.Reward_runes == t;
		}
	}
}
