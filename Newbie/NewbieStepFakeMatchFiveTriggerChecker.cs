using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveTriggerChecker : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveTriggerChecker()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_TriggerChecker;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.FakeMatchFiveStartCheckTrigger();
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
			NewbieManager.Instance.FakeMatchFiveCreateDelayStepEnd();
		}
	}
}
