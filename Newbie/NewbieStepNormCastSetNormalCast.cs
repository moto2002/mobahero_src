using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepNormCastSetNormalCast : NewbieStepBase
	{
		public NewbieStepNormCastSetNormalCast()
		{
			this._stepType = ENewbieStepType.NormCast_SetNormalCast;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideNormCastSetNormCast();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowNormCastSetNormCast();
		}
	}
}
