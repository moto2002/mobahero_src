using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieEleBatFiveCheckDelayHpLessThirtyEnd : MonoBehaviour
	{
		private float _delayTime = 10f;

		private float _startTime = Time.time;

		private void Update()
		{
			if (Time.time - this._startTime > this._delayTime)
			{
				base.enabled = false;
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirHpLessThirtyEnd, true, ENewbieStepType.EleBatFive_FirHpLessThirty);
			}
		}

		public void StartDelayMoveHpLessThirtyEnd(float inDelayTime)
		{
			this._delayTime = inDelayTime;
			this._startTime = Time.time;
		}

		public void StopDelayMoveHpLessThirtyEnd()
		{
			base.enabled = false;
		}
	}
}
