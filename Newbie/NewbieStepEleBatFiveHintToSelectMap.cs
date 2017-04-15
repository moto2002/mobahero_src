using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveHintToSelectMap : NewbieStepBase
	{
		public NewbieStepEleBatFiveHintToSelectMap()
		{
			this._stepType = ENewbieStepType.EleBatFive_HintToSelectMap;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.ElementaryBattleFiveFive);
			Singleton<NewbieView>.Instance.ShowHintEleBatFiveToSelectMap();
		}
	}
}
