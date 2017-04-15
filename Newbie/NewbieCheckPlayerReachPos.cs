using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieCheckPlayerReachPos : MonoBehaviour
	{
		private Vector3 _centerPos = Vector3.one;

		private float _rangeOffset = 1f;

		private Units _playerHero;

		private Vector3 _curPos = Vector3.one;

		public void InitInfo(Vector3 inCenterPos, float inRangeOffset)
		{
			this._centerPos = inCenterPos;
			this._rangeOffset = inRangeOffset;
		}

		private void Update()
		{
			this._playerHero = PlayerControlMgr.Instance.GetPlayer();
			if (this._playerHero != null && this._playerHero.mTransform != null)
			{
				this._curPos = this._playerHero.mTransform.position;
				if ((this._curPos - this._centerPos).sqrMagnitude < this._rangeOffset * this._rangeOffset)
				{
					NewbieManager.Instance.MoveNextStep();
					base.enabled = false;
				}
			}
		}
	}
}
