using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieSettlementContinueCtrl : MonoBehaviour
	{
		private float _delayTimeLength = 1f;

		private float _startDelayTime = 1f;

		private void Update()
		{
			if (Time.time - this._startDelayTime > this._delayTimeLength)
			{
				base.enabled = false;
				this.ShowSettlementContinue();
			}
		}

		private void ShowSettlementContinue()
		{
			Singleton<NewbieView>.Instance.ShowSettlementContinue();
		}

		public void StartDelayShowSettlementContinue(float inDelayTime)
		{
			this._delayTimeLength = inDelayTime;
			this._startDelayTime = Time.time;
		}

		public void StopDelayShowSettlementContinue()
		{
			base.enabled = false;
		}
	}
}
