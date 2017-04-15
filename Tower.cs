using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class Tower : Building
{
	private TowerAttackIndicator _towerIndicator;

	private DurationWoundManager _DurationWoundMgr;

	private int _towerPriority = 999999;

	private BaseBuildingController _towerController;

	private PlayEffectAction m_effectFangtouta;

	private string towerName = string.Empty;

	private float fWarningTimer = 2f;

	public int TowerPriority
	{
		get
		{
			return this._towerPriority;
		}
		set
		{
			this._towerPriority = value;
		}
	}

	protected override void OnInit(bool isRebirth = false)
	{
		SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(this.npc_id);
		string music_id = monsterMainData.music_id;
		AudioMgr.loadSoundBank_Skill("Tower", true, 0);
		base.OnInit(false);
	}

	protected override void OnCreate()
	{
		this.data = base.AddUnitComponent<MonsterDataManager>(!Singleton<PvpManager>.Instance.IsInPvp);
		this.atkController = base.AddUnitComponent<MonsterAttackController>();
		this._towerIndicator = TowerAttackIndicator.TryAddIndicator(this);
		this._DurationWoundMgr = base.AddUnitComponent<DurationWoundManager>();
		this._towerController = base.GetComponentInChildren<BaseBuildingController>();
		base.OnCreate();
		if (this._towerController)
		{
			this._towerController.OnCreate(this);
		}
	}

	protected override void OnUpdate(float delta)
	{
		base.UpdateCameraVisible();
		base.OnUpdate(delta);
		if (this.fWarningTimer >= 0f)
		{
			this.fWarningTimer -= delta;
		}
	}

	public override void Wound(Units attacker, float damage)
	{
		if (damage > 0f)
		{
			return;
		}
		if (this.fWarningTimer < 0f && attacker != null && attacker.isHero)
		{
			this.fWarningTimer = 2f;
			bool flag = false;
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits(attacker.TeamType, global::TargetTag.Monster);
			if (mapUnits != null)
			{
				for (int i = 0; i < mapUnits.Count; i++)
				{
					if (mapUnits[i] != null && (mapUnits[i].transform.position - base.transform.position).sqrMagnitude < 100f && TagManager.CheckTag(mapUnits[i], global::TargetTag.CreepsAndMinions) && mapUnits[i].UnitType != UnitType.SummonMonster)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				if (base.isLocalUnit)
				{
					UIMessageBox.ShowTowerWoundWarn(this);
					Singleton<MiniMapView>.Instance.ShowTowerMapWarn(this);
				}
				if (this.m_effectFangtouta != null && !this.m_effectFangtouta.isDestroyed)
				{
					this.m_effectFangtouta.Destroy();
				}
				if (base.TeamType == TeamType.LM)
				{
					this.m_effectFangtouta = ActionManager.PlayEffect("Fx_Fangtouta_LM", this, null, null, true, string.Empty, null);
				}
				else
				{
					this.m_effectFangtouta = ActionManager.PlayEffect("Fx_Fangtouta_BL", this, null, null, true, string.Empty, null);
				}
			}
		}
		base.Wound(attacker, damage);
		this._DurationWoundMgr.OnWound(attacker, damage);
		if (this._towerController)
		{
			this._towerController.OnDamage(attacker, damage);
		}
	}

	protected override void DeathIncome(Units attacker)
	{
		UtilManager.Instance.AddDeathGold(attacker, this);
		UtilManager.Instance.AddExp(attacker, this);
		UtilManager.Instance.UpdateKillState(attacker, this);
		Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitTowerDestroy, this, attacker, null);
		if (this.isHome)
		{
			AudioMgr.Play("Play_Crystal_Explosion", null, false, false);
		}
		if (this._towerController)
		{
			this._towerController.OnDead();
		}
	}

	public void SetCurAttackTarget(Units target)
	{
		if (this._towerIndicator != null)
		{
			this._towerIndicator.CurAttackTarget = target;
		}
	}

	public void Prewarm()
	{
		if (!this._towerController)
		{
			ClientLogger.Error("BaseBuildingController not found");
			return;
		}
		new Task(this._towerController.OnPrewarm(), true);
	}
}
