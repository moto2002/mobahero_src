using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatFiveSettlementSec : NewbieStepBase
	{
		public NewbieStepEleBatFiveSettlementSec()
		{
			this._stepType = ENewbieStepType.EleBatFive_SettlementSec;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideSettlementContinue();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.1f));
			Singleton<NewbieView>.Instance.ShowSettlementContinue();
		}
	}
}
