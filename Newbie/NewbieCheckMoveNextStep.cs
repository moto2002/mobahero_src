using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieCheckMoveNextStep : MonoBehaviour
	{
		private float _delayTime = 1f;

		private float _startCheckTime = Time.time;

		public void StartAutoMoveNextStepDelay(float inDelayTime)
		{
			this._delayTime = inDelayTime;
			this._startCheckTime = Time.time;
			base.enabled = true;
		}

		private void Update()
		{
			if (Time.time - this._startCheckTime > this._delayTime)
			{
				this.StopAutoMoveNextStepDelay();
				NewbieManager.Instance.MoveNextStep();
			}
		}

		private void StopAutoMoveNextStepDelay()
		{
			base.enabled = false;
			this._startCheckTime = Time.time + 1000f;
		}
	}
}
