using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneSelSoldier : NewbieStepBase
	{
		private string _voiceResId = "2016";

		private float _loopTime = 10f;

		private string _mainText = "点击目标，即可选中";

		public NewbieStepEleBatOneOneSelSoldier()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SelSoldier;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			NewbieManager.Instance.HideSelSoldierHint();
			Singleton<NewbieView>.Instance.HideTitle();
			Singleton<NewbieView>.Instance.HideSelSoldierChecker();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowSelSoldierChecker();
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			NewbieManager.Instance.ShowSelSoldierHint();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
