using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieCheckSpecialEnterBattleStat : MonoBehaviour
	{
		private float _delayedTime = 9f;

		private float _startCheckTime = Time.time;

		private ENewbieStepType _moveStep;

		private void Update()
		{
			if (Time.time - this._startCheckTime > this._delayedTime)
			{
				this.StopCheckSpecialEnterBattle();
				if (!NewbieManager.Instance.IsSpecialEnterBattleSuc() && this.IsSpecialEnterBattleStep())
				{
					NewbieManager.Instance.OnSpecialEnterBattleFailed();
				}
			}
		}

		private bool IsSpecialEnterBattleStep()
		{
			return this._moveStep == ENewbieStepType.ElementaryBatOneOne_SelectMap || this._moveStep == ENewbieStepType.EleBatFive_SelectMap;
		}

		public void StopCheckSpecialEnterBattle()
		{
			base.enabled = false;
		}

		public void StartCheckSpecialEnterBattle(float inDelayedTime, ENewbieStepType inMoveStep)
		{
			this._delayedTime = inDelayedTime;
			this._moveStep = inMoveStep;
			this._startCheckTime = Time.time;
		}
	}
}
