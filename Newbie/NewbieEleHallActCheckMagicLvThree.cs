using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieEleHallActCheckMagicLvThree : MonoBehaviour
	{
		private void Update()
		{
			if (Singleton<BottleSystemView>.Instance != null && Singleton<BottleSystemView>.Instance.NewbieEleHallActIsOwnLegnedBottle())
			{
				this.StopCheckMagicBottleLvThree();
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_MagicBottleTale, false, ENewbieStepType.None);
			}
		}

		public void StartCheckMagicBottleLvThree()
		{
		}

		public void StopCheckMagicBottleLvThree()
		{
			base.enabled = false;
		}
	}
}
