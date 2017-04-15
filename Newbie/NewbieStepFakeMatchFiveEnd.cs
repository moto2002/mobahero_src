using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveEnd : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveEnd()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_End;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			this.ClearResources();
		}

		private void ClearResources()
		{
			NewbieManager.Instance.ClearPhaseFakeMatchFiveResources();
		}
	}
}
