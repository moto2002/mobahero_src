using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class ActivityTip_common : MonoBehaviour
	{
		public RewardItemBase Info
		{
			get;
			set;
		}

		public virtual bool IsThisCom(ERewardType t)
		{
			return false;
		}

		public virtual void RefrehUI()
		{
		}
	}
}
