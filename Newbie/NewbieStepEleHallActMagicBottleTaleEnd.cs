using System;

namespace Newbie
{
	public class NewbieStepEleHallActMagicBottleTaleEnd : NewbieStepBase
	{
		public NewbieStepEleHallActMagicBottleTaleEnd()
		{
			this._stepType = ENewbieStepType.EleHallAct_MagicBottleTaleEnd;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.HideSubtitle();
		}
	}
}
