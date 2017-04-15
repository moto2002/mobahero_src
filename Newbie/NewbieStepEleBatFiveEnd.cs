using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveEnd : NewbieStepBase
	{
		public NewbieStepEleBatFiveEnd()
		{
			this._stepType = ENewbieStepType.EleBatFive_End;
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
			NewbieManager.Instance.ClearPhaseEleBatFiveResources();
		}
	}
}
