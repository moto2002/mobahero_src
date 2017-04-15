using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneStoreHint : NewbieStepBase
	{
		private string _voiceResId = "2007";

		private string _mainText = "如何获得金币？";

		private string _subText = "杀小兵、英雄、野怪，都可以获得金币";

		public NewbieStepEleBatOneOneStoreHint()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_StoreHint;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText, this._subText);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(7.5f);
		}
	}
}
