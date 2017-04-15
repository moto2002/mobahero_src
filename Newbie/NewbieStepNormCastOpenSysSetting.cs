using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepNormCastOpenSysSetting : NewbieStepBase
	{
		private string _mainText = "请切换使用普通施法，操作更精确";

		public NewbieStepNormCastOpenSysSetting()
		{
			this._stepType = ENewbieStepType.NormCast_OpenSysSetting;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.NormCastStopCheckLearnSkillFir();
			Singleton<NewbieView>.Instance.HideTitle();
			Singleton<NewbieView>.Instance.HideNormCastOpenSysSetting();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowNormCastOpenSysSetting();
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
		}
	}
}
