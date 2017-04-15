using Com.Game.Data;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class ActivityTip_normal : ActivityTip_common
	{
		public UITexture tex_card;

		public GameObject go_discount3;

		public GameObject go_discount7;

		public override void RefrehUI()
		{
			this.tex_card.mainTexture = ResourceManager.Load<Texture>(base.Info.BIcon, true, true, null, 0, false);
			this.RefreshUI_disCount();
		}

		private void RefreshUI_disCount()
		{
			if (base.Info.Discount)
			{
				SysCouponVo sysCouponVo = base.Info.ExtraData as SysCouponVo;
				if (sysCouponVo != null)
				{
					this.go_discount3.SetActive(3 == sysCouponVo.off_number);
					this.go_discount7.SetActive(7 == sysCouponVo.off_number);
				}
			}
			else
			{
				this.go_discount3.SetActive(false);
				this.go_discount7.SetActive(false);
			}
		}

		public override bool IsThisCom(ERewardType t)
		{
			return t != ERewardType.Reward_runes && t != ERewardType.Reward_summonerFrame && ERewardType.Reward_summonerHeadPortrait != t;
		}
	}
}
