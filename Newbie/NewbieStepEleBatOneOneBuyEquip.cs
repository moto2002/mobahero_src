using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneBuyEquip : NewbieStepBase
	{
		private string _voiceResId = "2009";

		private float _loopTime = 10f;

		private string _mainText = "快捷购买推荐装备";

		private string _subText = "快捷购买推荐装备，点击图标即可直接购买";

		public NewbieStepEleBatOneOneBuyEquip()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_BuyEquip;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideTitle();
			Singleton<NewbieView>.Instance.HideFastBuyEquipHint();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowFastBuyEquipHint();
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText, this._subText);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
