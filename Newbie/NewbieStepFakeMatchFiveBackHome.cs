using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveBackHome : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveBackHome()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_BackHome;
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
