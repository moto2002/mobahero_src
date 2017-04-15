using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveOpenSummonerSkill : NewbieStepBase
	{
		private string _voiceResId = "2207";

		private float _loopTime = 10f;

		public NewbieStepFakeMatchFiveOpenSummonerSkill()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_OpenSummonerSkill;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveOpenSummSkill();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveOpenSummSkill();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
