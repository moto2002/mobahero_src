using System;

namespace Newbie
{
	public class NewbieStepNormCastEnd : NewbieStepBase
	{
		public NewbieStepNormCastEnd()
		{
			this._stepType = ENewbieStepType.NormCast_End;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.ClearPhaseNormCastResources();
		}
	}
}
