using System;

namespace Newbie
{
	public class NewbieStepBatOneOneEnd : NewbieStepBase
	{
		public NewbieStepBatOneOneEnd()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_End;
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
			NewbieManager.Instance.ClearPhaseEleBatOneOneResources();
		}
	}
}
