using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFivePlayerSelectOneWay : NewbieStepBase
	{
		private string _mainText = "请根据指引，选择一条您想前往的路线吧！";

		private string _voiceResId = "2105";

		private float _loopTime = 10f;

		public NewbieStepEleBatFivePlayerSelectOneWay()
		{
			this._stepType = ENewbieStepType.EleBatFive_PlayerSelectOneWay;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EleBatFiveStopCheckSelOneWay();
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.EleBatFiveStartCheckSelOneWay();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
