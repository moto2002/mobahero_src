using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepNormCastCloseSysSetting : NewbieStepBase
	{
		public NewbieStepNormCastCloseSysSetting()
		{
			this._stepType = ENewbieStepType.NormCast_CloseSysSetting;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideNormCastCloseSysSetting();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowNormCastCloseSysSetting();
		}
	}
}
