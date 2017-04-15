using System;

namespace Newbie
{
	public abstract class NewbieStepBase
	{
		protected ENewbieStepType _stepType;

		protected NewbieStepBase _nextStep;

		public void DescribeSelf()
		{
		}

		public ENewbieStepType GetStepType()
		{
			return this._stepType;
		}

		public bool IsStep(ENewbieStepType inStepType)
		{
			return this._stepType == inStepType;
		}

		public void BindNextStep(NewbieStepBase inStep)
		{
			this._nextStep = inStep;
		}

		public NewbieStepBase GetNextStep()
		{
			return this._nextStep;
		}

		public void AutoMoveNextStepWithDelay(float inDelayTime)
		{
			NewbieManager.Instance.AutoMoveNextStepWithDelay(inDelayTime);
		}

		public abstract void OnLeave();

		public abstract void HandleAction();
	}
}
