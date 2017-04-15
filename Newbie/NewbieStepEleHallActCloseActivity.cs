using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActCloseActivity : NewbieStepBase
	{
		public NewbieStepEleHallActCloseActivity()
		{
			this._stepType = ENewbieStepType.EleHallAct_CloseActivity;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideEleHallActCloseActivity();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActCloseActivity();
		}
	}
}
