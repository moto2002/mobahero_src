using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneOpenStore : NewbieStepBase
	{
		private string _voiceResId = "2006";

		private float _loopTime = 10f;

		private string _mainText = "开启战斗内商店";

		private string _subText = "在商店附近时，点击左下角打开商店";

		public NewbieStepEleBatOneOneOpenStore()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_OpenStore;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideTitle();
			Singleton<NewbieView>.Instance.HideOpenStoreHint();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowOpenStoreHint();
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText, this._subText);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
