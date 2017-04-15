using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieCheckInTowerAtkRange : MonoBehaviour
	{
		private bool _isDoCheck;

		private Units _playerHero;

		private Vector3 _towerPos = Vector3.one;

		private float _towerAtkRange = 1f;

		private void Update()
		{
			if (!this._isDoCheck)
			{
				return;
			}
			if (this._playerHero == null || this._playerHero.mTransform == null)
			{
				return;
			}
			if ((this._playerHero.mTransform.position - this._towerPos).sqrMagnitude < this._towerAtkRange * this._towerAtkRange)
			{
				NewbieManager.Instance.MoveNextStep();
				base.enabled = false;
			}
		}

		public void StartCheckInTowerAtkRange(Units inHero, Vector3 inTowerPos, float inAtkRange)
		{
			this._isDoCheck = true;
			this._playerHero = inHero;
			this._towerPos = inTowerPos;
			this._towerAtkRange = inAtkRange;
		}

		public void StopCheckInTowerAtkRange()
		{
			base.enabled = false;
		}
	}
}
