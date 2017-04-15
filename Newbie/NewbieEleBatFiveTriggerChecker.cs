using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Newbie
{
	public class NewbieEleBatFiveTriggerChecker : MonoBehaviour
	{
		private GameObject _seeSoldierTowerHintPrefab;

		private Units _playerHero;

		private bool _isTriggerFirSeeEnemySoldier;

		private float _checkFirSeeEnemySoldierTimeLen = 0.8f;

		private float _lastCheckFirSeeEnemySoldierTime;

		private List<int> _firSeeEnemySoldierIds = new List<int>();

		private bool _isTriggerFirSeeEnemyTower;

		private float _checkFirSeeEnemyTowerTimeLen = 0.9f;

		private float _lastCheckFirSeeEnemyTowerTime;

		private int _firSeeEnemyTowerId;

		private bool _isTriggerFirInEnemyTowerRange;

		private float _checkFirInEnemyTowerRangeTimeLen = 0.8f;

		private float _lastCheckFirInEnemyTowerRangeTime;

		private bool _isTriggerFirHpLessEighty;

		private float _checkFirHpLessEightyTimeLen = 0.8f;

		private float _lastCheckFirHpLessEightyTime;

		private bool _isTriggerFirHpLessThirty;

		private float _checkFirHpLessThirtyTimeLen = 0.8f;

		private float _lastCheckFirHpLessThirtyTime;

		private bool _isTriggerFirNearOutShop;

		private float _checkFirNearOutShopTimeLen = 0.8f;

		private float _lastCheckFirNearOutShopTime;

		private Vector3 _outShopFirPos = new Vector3(-48.4f, 0f, 48.97f);

		private Vector3 _outShopSecPos = new Vector3(48.02f, 0f, -49.48f);

		private float _nearOutShopDistSqr = 100f;

		private bool _isTriggerFirOverMoney;

		private float _checkFirOverMoneyTimeLen = 2f;

		private float _lastCheckFirOverMoneyTime;

		private PvpStatisticMgr.HeroData _playerHeroData;

		private bool _isTriggerFirInSpring;

		private float _checkFirInSpringTimeLen = 0.8f;

		private float _lastCheckFirInSpringTime;

		private Vector3 _enemySpringPos = new Vector3(50.65f, 0f, 50.69f);

		private float _inEnemySpringDistSqr = 81.5f;

		private bool _isTriggerLineTowersDead;

		private float _checkLineTowersDeadTimeLen = 0.8f;

		private float _lastCheckLineTowersDeadTime;

		private bool _isTriggerFirNearGrass;

		private int _checkGrassAreaIdx;

		private float _playerGrassDistX = 1f;

		private float _playerGrassDistZ = 1f;

		private float _grassRange = 1f;

		private Vector3[] _grassAreas = new Vector3[]
		{
			new Vector3(-51.3f, 7.65f, 4.5f),
			new Vector3(-51.145f, 40.086f, 4.5f),
			new Vector3(-39.9f, 51.16f, 4.5f),
			new Vector3(-33.08f, 31.39f, 6.1f),
			new Vector3(-7.61f, 48.5f, 4.5f),
			new Vector3(13.97f, 37.43f, 4f),
			new Vector3(-17.52f, 29.63f, 6.2f),
			new Vector3(9.75f, 19.79f, 6f),
			new Vector3(-6.8f, 6.14f, 7.1f),
			new Vector3(6.48f, -7.08f, 7.1f),
			new Vector3(-35.92f, 9.26f, 6f),
			new Vector3(-29.77f, -16.05f, 6f),
			new Vector3(-9.07f, -21.21f, 6f),
			new Vector3(-15.06f, -36.91f, 6f),
			new Vector3(-16.74f, -29.69f, 6f),
			new Vector3(33.46f, -33.33f, 6.1f),
			new Vector3(24.46f, -11.42f, 4f),
			new Vector3(34.29f, -13.06f, 4.9f),
			new Vector3(50.03f, -9.2f, 4.9f),
			new Vector3(51.86f, -39.48f, 4.5f),
			new Vector3(40.6f, -51.01f, 4.5f),
			new Vector3(8.43f, -49.74f, 4.9f),
			new Vector3(28.35f, 13.89f, 4f)
		};

		private bool _isTriggerAllLearnSkill;

		private int _heroPreLevel;

		private int _heroCurLevel;

		private float _checkLearnSkillTimeLen = 0.5f;

		private float _lastCheckLearnSkillTime;

		private bool _isCheckFirSeeEnemySoldierAllDead;

		private float _checkFirSeeEnemySoldierDeadTimeLen = 1f;

		private float _lastCheckFirSeeEnemySoldierDeadTime;

		private bool _isCheckFirSeeEnemyTowerDead;

		private float _checkFirSeeEnemyTowerDeadTimeLen = 1f;

		private float _lastCheckFirSeeEnemyTowerDeadTime;

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
			if (NewbieManager.Instance.IsEnableEleBatFiveCheckTrigger())
			{
				if (this.TryTriggerFirSeeEnemySoldier())
				{
					return;
				}
				if (this.TryTriggerFirSeeEnemyTower())
				{
					return;
				}
				if (this.TryTriggerFirInEnemyTowerRange())
				{
					return;
				}
				if (this.TryTriggerFirHpLessEighty())
				{
					return;
				}
				if (this.TryTriggerFirHpLessThirty())
				{
					return;
				}
				if (this.TryTriggerFirNearOutShop())
				{
					return;
				}
				if (this.TryTriggerFirOverMoney())
				{
					return;
				}
				if (this.TryTriggerFirInSpring())
				{
					return;
				}
				if (this.TryTriggerLineTowersDead())
				{
					return;
				}
				if (this.TryTriggerFirNearGrass())
				{
					return;
				}
				if (this.TryTriggerLearnSkill())
				{
					return;
				}
			}
			this.TryCheckFirSeeEnemySoldierAllDead();
			this.TryCheckFirSeeEnemyTowerDead();
		}

		private bool TryTriggerFirSeeEnemySoldier()
		{
			if (this._isTriggerFirSeeEnemySoldier)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirSeeEnemySoldierTime > this._checkFirSeeEnemySoldierTimeLen)
			{
				this._lastCheckFirSeeEnemySoldierTime = Time.time;
				if (MapManager.Instance.GetPlayerSeeEnemySoldierInfo(this.CachedPlayerHero, this._firSeeEnemySoldierIds, this._seeSoldierTowerHintPrefab))
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirSeeEnemySoldier, false, ENewbieStepType.None);
					this._isCheckFirSeeEnemySoldierAllDead = true;
					this._isTriggerFirSeeEnemySoldier = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirSeeEnemyTower()
		{
			if (this._isTriggerFirSeeEnemyTower)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirSeeEnemyTowerTime > this._checkFirSeeEnemyTowerTimeLen)
			{
				this._lastCheckFirSeeEnemyTowerTime = Time.time;
				if (MapManager.Instance.GetPlayerSeeEnemyTowerInfo(this._seeSoldierTowerHintPrefab, out this._firSeeEnemyTowerId))
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirSeeEnemyTower, false, ENewbieStepType.None);
					this._isCheckFirSeeEnemyTowerDead = true;
					this._isTriggerFirSeeEnemyTower = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirInEnemyTowerRange()
		{
			if (this._isTriggerFirInEnemyTowerRange)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirInEnemyTowerRangeTime > this._checkFirInEnemyTowerRangeTimeLen)
			{
				this._lastCheckFirInEnemyTowerRangeTime = Time.time;
				if (MapManager.Instance.CheckPlayerInTowerAtkRange(this.CachedPlayerHero))
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirInEnemyTowerRange, false, ENewbieStepType.None);
					this._isTriggerFirInEnemyTowerRange = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirHpLessEighty()
		{
			if (this._isTriggerFirHpLessEighty)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirHpLessEightyTime > this._checkFirHpLessEightyTimeLen)
			{
				this._lastCheckFirHpLessEightyTime = Time.time;
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.hp < this.CachedPlayerHero.hp_max * 0.8f)
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirHpLessEighty, false, ENewbieStepType.None);
					this._isTriggerFirHpLessEighty = true;
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
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.hp < this.CachedPlayerHero.hp_max * 0.39f)
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirHpLessThirty, false, ENewbieStepType.None);
					this._isTriggerFirHpLessThirty = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirNearOutShop()
		{
			if (this._isTriggerFirNearOutShop)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirNearOutShopTime > this._checkFirNearOutShopTimeLen)
			{
				this._lastCheckFirNearOutShopTime = Time.time;
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.mTransform != null && ((this.CachedPlayerHero.mTransform.position - this._outShopFirPos).sqrMagnitude < this._nearOutShopDistSqr || (this.CachedPlayerHero.mTransform.position - this._outShopSecPos).sqrMagnitude < this._nearOutShopDistSqr))
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirNearOutShop, false, ENewbieStepType.None);
					this._isTriggerFirNearOutShop = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirOverMoney()
		{
			if (this._isTriggerFirOverMoney)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirOverMoneyTime > this._checkFirOverMoneyTimeLen)
			{
				this._lastCheckFirOverMoneyTime = Time.time;
				if (this.CachedPlayerHero != null && Singleton<PvpManager>.Instance.StatisticMgr != null)
				{
					this._playerHeroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(this.CachedPlayerHero.unique_id);
					if (this._playerHeroData != null && this._playerHeroData.CurGold > 1000)
					{
						NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirOverMoney, false, ENewbieStepType.None);
						this._isTriggerFirOverMoney = true;
						return true;
					}
				}
			}
			return false;
		}

		private bool TryTriggerFirInSpring()
		{
			if (this._isTriggerFirInSpring)
			{
				return false;
			}
			if (Time.time - this._lastCheckFirInSpringTime > this._checkFirInSpringTimeLen)
			{
				this._lastCheckFirInSpringTime = Time.time;
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.mTransform != null && (this.CachedPlayerHero.mTransform.position - this._enemySpringPos).sqrMagnitude < this._inEnemySpringDistSqr)
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirInEnemySpring, false, ENewbieStepType.None);
					this._isTriggerFirInSpring = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerLineTowersDead()
		{
			if (this._isTriggerLineTowersDead)
			{
				return false;
			}
			if (Time.time - this._lastCheckLineTowersDeadTime > this._checkLineTowersDeadTimeLen)
			{
				this._lastCheckLineTowersDeadTime = Time.time;
				if (MapManager.Instance.BatFiveCheckLineTowersDead())
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_LineTowersDead, false, ENewbieStepType.None);
					this._isTriggerLineTowersDead = true;
					return true;
				}
			}
			return false;
		}

		private bool TryTriggerFirNearGrass()
		{
			if (this._isTriggerFirNearGrass)
			{
				return false;
			}
			if (this._checkGrassAreaIdx >= 0 && this._checkGrassAreaIdx < this._grassAreas.Length)
			{
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.mTransform != null)
				{
					this._playerGrassDistX = this.CachedPlayerHero.mTransform.position.x - this._grassAreas[this._checkGrassAreaIdx].x;
					this._playerGrassDistZ = this.CachedPlayerHero.mTransform.position.z - this._grassAreas[this._checkGrassAreaIdx].y;
					this._grassRange = this._grassAreas[this._checkGrassAreaIdx].z;
					if (this._playerGrassDistX * this._playerGrassDistX + this._playerGrassDistZ * this._playerGrassDistZ < this._grassRange * this._grassRange)
					{
						NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirNearGrass, false, ENewbieStepType.None);
						this._isTriggerFirNearGrass = true;
						return true;
					}
					this._checkGrassAreaIdx++;
				}
			}
			else
			{
				this._checkGrassAreaIdx = 0;
			}
			return false;
		}

		private bool TryTriggerLearnSkill()
		{
			if (this._isTriggerAllLearnSkill)
			{
				return false;
			}
			if (Time.time - this._lastCheckLearnSkillTime > this._checkLearnSkillTimeLen)
			{
				this._lastCheckLearnSkillTime = Time.time;
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.skillManager != null && this.CachedPlayerHero.skillManager.SkillPointsLeft > 0)
				{
					this._heroCurLevel = this.CachedPlayerHero.level;
					if (this._heroCurLevel != this._heroPreLevel)
					{
						this._heroPreLevel = this._heroCurLevel;
						if (this._heroCurLevel == 1)
						{
							NewbieManager.Instance.ShowEleBatFiveLearnSkillHint(0);
							return true;
						}
						if (this._heroCurLevel == 2)
						{
							NewbieManager.Instance.ShowEleBatFiveLearnSkillHint(1);
							return true;
						}
						if (this._heroCurLevel == 3)
						{
							NewbieManager.Instance.ShowEleBatFiveLearnSkillHint(2);
							return true;
						}
						if (this._heroCurLevel == 4)
						{
							NewbieManager.Instance.ShowEleBatFiveLearnSkillHint(0);
							return true;
						}
						if (this._heroCurLevel == 5)
						{
							NewbieManager.Instance.ShowEleBatFiveLearnSkillHint(1);
							return true;
						}
						if (this._heroCurLevel == 6)
						{
							NewbieManager.Instance.ShowEleBatFiveLearnSkillHint(3);
							this._isTriggerAllLearnSkill = true;
							return true;
						}
						this._isTriggerAllLearnSkill = true;
						return true;
					}
				}
			}
			return false;
		}

		private void TryCheckFirSeeEnemySoldierAllDead()
		{
			if (!this._isCheckFirSeeEnemySoldierAllDead)
			{
				return;
			}
			if (Time.time - this._lastCheckFirSeeEnemySoldierDeadTime > this._checkFirSeeEnemySoldierDeadTimeLen)
			{
				this._lastCheckFirSeeEnemySoldierDeadTime = Time.time;
				if (MapManager.Instance.CheckEnemySoldierAllDead(this._firSeeEnemySoldierIds))
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirSeeEnemySoldierEnd, false, ENewbieStepType.None);
					this._isCheckFirSeeEnemySoldierAllDead = false;
				}
			}
		}

		private void TryCheckFirSeeEnemyTowerDead()
		{
			if (!this._isCheckFirSeeEnemyTowerDead)
			{
				return;
			}
			if (Time.time - this._lastCheckFirSeeEnemyTowerDeadTime > this._checkFirSeeEnemyTowerDeadTimeLen)
			{
				this._lastCheckFirSeeEnemyTowerDeadTime = Time.time;
				if (MapManager.Instance.CheckEnemyTowerDead(this._firSeeEnemyTowerId))
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirSeeEnemyTowerEnd, false, ENewbieStepType.None);
					this._isCheckFirSeeEnemyTowerDead = false;
				}
			}
		}

		public void StartCheckTrigger()
		{
			this._seeSoldierTowerHintPrefab = ResourceManager.LoadPath<GameObject>("Prefab/Effects/CommonEffect/Fx_attack_point", null, 0);
		}

		public void StopCheckTrigger()
		{
			base.enabled = false;
			this._seeSoldierTowerHintPrefab = null;
		}
	}
}
