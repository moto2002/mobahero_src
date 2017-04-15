using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieEleBatFiveCheckSelWay : MonoBehaviour
	{
		private Vector3 _upPos = new Vector3(-48.28f, 0f, -18.29f);

		private Vector3 _midPos = new Vector3(-26.77f, 0f, -26.64f);

		private Vector3 _downPos = new Vector3(-17.53f, 0f, -48f);

		private float _checkRadiusSqr = 49f;

		private Units _playerHero;

		private Vector3 _playerPos = Vector3.one;

		private Units CachedPlayerHero
		{
			get
			{
				if (this._playerHero == null && PlayerControlMgr.Instance != null)
				{
					this._playerHero = PlayerControlMgr.Instance.GetPlayer();
				}
				return this._playerHero;
			}
		}

		private void Update()
		{
			if (this.CachedPlayerHero != null && this.CachedPlayerHero.mTransform != null)
			{
				this._playerPos = this.CachedPlayerHero.mTransform.position;
				if ((this._playerPos - this._upPos).sqrMagnitude < this._checkRadiusSqr)
				{
					base.enabled = false;
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_PlayerSelectWayEnd, false, ENewbieStepType.None);
				}
				if ((this._playerPos - this._midPos).sqrMagnitude < this._checkRadiusSqr)
				{
					base.enabled = false;
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_PlayerSelectWayEnd, false, ENewbieStepType.None);
				}
				if ((this._playerPos - this._downPos).sqrMagnitude < this._checkRadiusSqr)
				{
					base.enabled = false;
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_PlayerSelectWayEnd, false, ENewbieStepType.None);
				}
			}
		}

		public void StartCheckSelWay()
		{
		}

		public void StopCheckSelWay()
		{
			base.enabled = false;
		}
	}
}
