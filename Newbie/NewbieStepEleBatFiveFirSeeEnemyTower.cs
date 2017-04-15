using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirSeeEnemyTower : NewbieStepBase
	{
		private string _mainText = "这是一个推塔的游戏！";

		private string _subText = "摧毁敌方防御塔，向胜利推进！";

		private string _voiceResId = "2107";

		private float _loopTime = 10f;

		public NewbieStepEleBatFiveFirSeeEnemyTower()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirSeeEnemyTower;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
			NewbieManager.Instance.DestroyEleBatFiveFirSeeTowerHintObj();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText, this._subText);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
