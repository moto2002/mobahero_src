using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveFirNearFirTower : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveFirNearFirTower()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_FirNearFirTower;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
			NewbieManager.Instance.StopFakeMatchFiveCheckFirNearFirTowerEnd();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveBarrage();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableFakeMatchFiveTrigger();
			NewbieManager.Instance.StartFakeMatchFiveCheckFirNearFirTowerEnd();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveBarrage();
		}
	}
}
