using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveFirHeroDead : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveFirHeroDead()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_FirHeroDead;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
			NewbieManager.Instance.StopFakeMatchFiveCheckFirHeroDeadEnd();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveHintAttack();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableFakeMatchFiveTrigger();
			NewbieManager.Instance.StartFakeMatchFiveCheckFirHeroDeadEnd();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveHintAttack();
		}
	}
}
