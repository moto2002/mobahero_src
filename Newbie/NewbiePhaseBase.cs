using System;
using System.Collections.Generic;

namespace Newbie
{
	public abstract class NewbiePhaseBase
	{
		protected ENewbiePhaseType _phaseType;

		protected NewbiePhaseBase _nextPhase;

		protected NewbieStepBase _curStep;

		protected List<NewbieStepBase> _allSteps = new List<NewbieStepBase>();

		public NewbiePhaseBase()
		{
		}

		public void DescribeSelf()
		{
		}

		public void BindNextPhase(NewbiePhaseBase inPhase)
		{
			this._nextPhase = inPhase;
		}

		public NewbiePhaseBase GetNextPhase()
		{
			return this._nextPhase;
		}

		public ENewbiePhaseType GetPhaseType()
		{
			return this._phaseType;
		}

		public bool IsPhase(ENewbiePhaseType inPhaseType)
		{
			return this._phaseType == inPhaseType;
		}

		public bool IsCurStep(ENewbieStepType inStepType)
		{
			return this._curStep != null && this._curStep.IsStep(inStepType);
		}

		public abstract void InitSteps();

		public abstract void EnterPhase();

		public NewbieStepBase FindCertainStep(ENewbieStepType inStepType)
		{
			for (int i = 0; i < this._allSteps.Count; i++)
			{
				NewbieStepBase newbieStepBase = this._allSteps[i];
				if (newbieStepBase.IsStep(inStepType))
				{
					return newbieStepBase;
				}
			}
			return null;
		}

		public void MoveNextStep()
		{
			if (this._curStep == null)
			{
				return;
			}
			NewbieStepBase nextStep = this._curStep.GetNextStep();
			if (nextStep == null)
			{
				return;
			}
			this._curStep.OnLeave();
			nextStep.HandleAction();
			this._curStep = nextStep;
			this._curStep.DescribeSelf();
		}

		public bool MoveCertainStep(ENewbieStepType inStepType, bool inIsCheckCurStep, ENewbieStepType inCheckStepType)
		{
			if (inIsCheckCurStep && this._curStep.GetStepType() != inCheckStepType)
			{
				return false;
			}
			NewbieStepBase newbieStepBase = this.FindCertainStep(inStepType);
			if (newbieStepBase == null)
			{
				return false;
			}
			if (this._curStep.GetStepType() == newbieStepBase.GetStepType())
			{
				return false;
			}
			this._curStep.OnLeave();
			newbieStepBase.HandleAction();
			this._curStep = newbieStepBase;
			this._curStep.DescribeSelf();
			return true;
		}
	}
}
