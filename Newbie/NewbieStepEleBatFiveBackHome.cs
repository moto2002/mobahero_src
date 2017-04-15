using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatFiveBackHome : NewbieStepBase
	{
		public NewbieStepEleBatFiveBackHome()
		{
			this._stepType = ENewbieStepType.EleBatFive_BackHome;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideSettlementBackHome();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.1f));
			Singleton<NewbieView>.Instance.ShowSettlementBackHome();
		}
	}
}
