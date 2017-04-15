using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieFakeMatchFiveTriggerChecker : MonoBehaviour
	{
		private Units _playerHero;

		private Vector3 _playerPos = Vector3.zero;

		private bool _isTriggerFirHpLessNinty;

		private float _checkFirHpLessNintyTimeLen = 0.8f;

		private float _lastCheckFirHpLessNintyTime;

		private bool _isTriggerFirHpLessThirty;

		private float _checkFirHpLessThirtyTimeLen = 0.8f;

		private float _lastCheckFirHpLessThirtyTime;

		private bool _isTriggerSelUpOrDownWay;

		private Vector3 _upWayPos = new Vector3(-48.28f, 0f, -18.29f);

		private Vector3 _downWayPos = new Vector3(-17.53f, 0f, -48f);

		private Vector3 _platCenterPos = new Vector3(-50f, 0f, -50f);

		private float _checkWayRadiusSqr = 1122.25f;

		private bool _isTriggerFirHeroDead;

		private bool _isTriggerFirNearFirTower;

		private Vector3 _firTowerUpPos = new Vector3(-48.99f, 0f, 18.87f);

		private Vector3 _firTowerMidPos = new Vector3(-12.21f, 0f, -9.22f);

		private Vector3 _firTowerDownPos = new Vector3(18.88f, 0f, -49.13f);

		private float _checkFirNearFirTowerTimeLen = 0.1f;

		private float _lastCheckFirNearFirTowerTime;

		private bool _isFinUseRecoveryHintOnDead;

		private bool _isFinUseBackHomeHintOnDead;

		private bool _isSuc;

		private bool _isPlayerLive;

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
			if (NewbieManager.Instance.IsEnableFakeMatchFiveTrigger())
			{
				if (this.TryTriggerFirHpLessNinty())
				{
					return;
				}
				if (this.TryTriggerFirHpLessThirty())
				{
					return;
				}
				if (this.TryTriggerFirSelUpOrDownWay())
				{
					return;
				}
				if (this.TryTriggerFirHeroDead())
				{
					return;
				}
				if (this.TryTriggerFirNearFirTower())
				{
					return;
				}
			}
			this.TryFinStepOnPlayerDead();
		}

		private bool TryTriggerFirHpLessNinty()
		{
			if (this._isTriggerFirHpLessNinty)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirHpLessNintyTime > this._checkFirHpLessNintyTimeLen)
			{
				this._lastCheckFirHpLessNintyTime = Time.time;
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.isLive && this.CachedPlayerHero.hp < this.CachedPlayerHero.hp_max * 0.9f)
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHpLessNinty, false, ENewbieStepType.None);
					this._isTriggerFirHpLessNinty = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirHpLessThirty()
		{
			if (this._isTriggerFirHpLessThirty)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirHpLessThirtyTime > this._checkFirHpLessThirtyTimeLen)
			{
				this._lastCheckFirHpLessThirtyTime = Time.time;
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.isLive && this.CachedPlayerHero.hp < this.CachedPlayerHero.hp_max * 0.39f)
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHpLessThirty, false, ENewbieStepType.None);
					this._isTriggerFirHpLessThirty = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirSelUpOrDownWay()
		{
			if (this._isTriggerSelUpOrDownWay)
			{
				return false;
			}
			if (this.CachedPlayerHero != null && this.CachedPlayerHero.mTransform != null)
			{
				this._playerPos = this.CachedPlayerHero.mTransform.position;
				if ((this._playerPos - this._platCenterPos).sqrMagnitude > this._checkWayRadiusSqr)
				{
					this._isTriggerSelUpOrDownWay = true;
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirSelDownWay, false, ENewbieStepType.None);
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirHeroDead()
		{
			if (this._isTriggerFirHeroDead)
			{
				return false;
			}
			if (this.CachedPlayerHero != null && !this.CachedPlayerHero.isLive)
			{
				this._isTriggerFirHeroDead = true;
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHeroDead, false, ENewbieStepType.None);
				return true;
			}
			return false;
		}

		private bool TryTriggerFirNearFirTower()
		{
			if (this._isTriggerFirNearFirTower)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirNearFirTowerTime > this._checkFirNearFirTowerTimeLen)
			{
				this._lastCheckFirNearFirTowerTime = Time.time;
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.mTransform != null)
				{
					this._playerPos = this.CachedPlayerHero.mTransform.position;
					if ((this._playerPos - this._firTowerUpPos).sqrMagnitude < 9f)
					{
						this._isTriggerFirNearFirTower = true;
						NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirNearFirTower, false, ENewbieStepType.None);
						return true;
					}
					if ((this._playerPos - this._firTowerMidPos).sqrMagnitude < 9f)
					{
						this._isTriggerFirNearFirTower = true;
						NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirNearFirTower, false, ENewbieStepType.None);
						return true;
					}
					if ((this._playerPos - this._firTowerDownPos).sqrMagnitude < 9f)
					{
						this._isTriggerFirNearFirTower = true;
						NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirNearFirTower, false, ENewbieStepType.None);
						return true;
					}
				}
			}
			return false;
		}

		private void TryFinStepOnPlayerDead()
		{
			if (this._isFinUseRecoveryHintOnDead && this._isFinUseBackHomeHintOnDead)
			{
				return;
			}
			if (this.CachedPlayerHero == null)
			{
				return;
			}
			if (this._isPlayerLive && !this.CachedPlayerHero.isLive)
			{
				if (!this._isFinUseRecoveryHintOnDead)
				{
					this._isSuc = NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHpLessNintyEnd, true, ENewbieStepType.FakeMatchFive_FirHpLessNinty);
					if (this._isSuc)
					{
						this._isFinUseRecoveryHintOnDead = true;
					}
				}
				if (!this._isFinUseBackHomeHintOnDead)
				{
					this._isSuc = NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHpLessThirtyEnd, true, ENewbieStepType.FakeMatchFive_FirHpLessThirty);
					if (this._isSuc)
					{
						this._isFinUseBackHomeHintOnDead = true;
					}
				}
			}
			this._isPlayerLive = this.CachedPlayerHero.isLive;
		}
	}
}
