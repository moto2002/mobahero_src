using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveSkillPanelSet : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveSkillPanelSet()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_SkillPanelSet;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableFakeMatchFiveTrigger();
			if (Singleton<ReturnView>.Instance != null)
			{
				Singleton<ReturnView>.Instance.TryShowSetSkillPivotHint();
			}
		}
	}
}
