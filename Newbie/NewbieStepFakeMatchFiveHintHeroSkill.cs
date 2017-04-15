using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveHintHeroSkill : NewbieStepBase
	{
		private string _voiceResId = "2206";

		private float _loopTime = 10f;

		public NewbieStepFakeMatchFiveHintHeroSkill()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_HintHeroSkill;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveHeroSkillIntro();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveHeroSkillIntro();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
