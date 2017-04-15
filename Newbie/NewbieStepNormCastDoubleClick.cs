using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepNormCastDoubleClick : NewbieStepBase
	{
		private string _mainText = "双击技能图标可快速释放技能";

		public NewbieStepNormCastDoubleClick()
		{
			this._stepType = ENewbieStepType.NormCast_DoubleClick;
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
			base.AutoMoveNextStepWithDelay(2f);
		}
	}
}
