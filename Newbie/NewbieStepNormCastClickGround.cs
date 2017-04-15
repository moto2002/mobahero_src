using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepNormCastClickGround : NewbieStepBase
	{
		private string _mainText = "请点击地面，即可释放技能";

		public NewbieStepNormCastClickGround()
		{
			this._stepType = ENewbieStepType.NormCast_ClickGround;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
		}
	}
}
