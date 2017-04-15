using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirInEnemyTowerRange : NewbieStepBase
	{
		private string _mainText = "慎重选择越塔杀敌！";

		private string _subText = "在敌方防御塔下攻击敌方英雄时会受到防御塔的攻击";

		private string _voiceResId = "2108";

		public NewbieStepEleBatFiveFirInEnemyTowerRange()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirInEnemyTowerRange;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopVoiceHintOnce();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText, this._subText);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(5.5f);
		}
	}
}
