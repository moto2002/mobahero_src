using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaHeros.Pvp;
using Newbie;
using System;
using UnityEngine;

public class Entity : Units
{
	protected new bool isMovingEntity;

	protected override void OnCreate()
	{
		base.OnCreate();
		base.SetUnitsLifeTime(UnitsLifeTime.Default);
		if (base.tag.Equals("Player") || base.tag.Equals("Hero"))
		{
			this.crickTime = base.AddUnitComponent<CrickTime>();
		}
		this.timeSyncSystem = base.AddUnitComponent<UnitsTimeSyncSystem>();
		this.dataChange = base.AddUnitComponent<DataChangeManager>();
		this.statistics = base.AddUnitComponent<StatisticsManager>();
		this.unitCollider = base.AddUnitComponent<UnitCollider>();
		this.surface = base.AddUnitComponent<SurfaceManager>();
		this.animController = base.AddUnitComponent<AnimController>();
		this.effectManager = base.AddUnitComponent<EffectManager>();
		this.skillManager = base.AddUnitComponent<SkillManager>();
		this.highEffManager = base.AddUnitComponent<HighEffManager>();
		base.ImmunityManager = base.AddUnitComponent<ImmunityManager>();
		if (this.moveController != null)
		{
			this.moveController.isMovingEntity = this.isMovingEntity;
		}
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			this.buffManager = base.AddUnitComponent<PVP_BuffManager>();
		}
		else
		{
			this.buffManager = base.AddUnitComponent<BuffManager>();
		}
		base.SetUnitsLifeTime(UnitsLifeTime.BeforeInit);
	}

	protected override void OnInit(bool isRebirth = false)
	{
		base.ClearAllCharaState();
		base.OnInit(isRebirth);
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.CleanAllData);
		base.SetUnitsLifeTime(UnitsLifeTime.BeforeStart);
		if (this.isPlayer)
		{
			Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.PlayerInit);
		}
	}

	protected override void OnStart()
	{
		base.OnStart();
		base.SetUnitsLifeTime(UnitsLifeTime.InGame);
	}

	protected override void OnStop()
	{
		base.OnStop();
	}

	protected override void OnEnd()
	{
		if (!base.isLive)
		{
			return;
		}
		base.EnableAction(false);
		base.ResetState();
		this.m_CoroutineManager.StopAllCoroutine();
		base.StopAllUnitComponent();
		base.SetUnitsLifeTime(UnitsLifeTime.Stop);
	}

	public override void Wound(Units attacker, float damage)
	{
		if (damage > 0f)
		{
			return;
		}
		if (base.BoZang.IsInState && base.hp <= 0f)
		{
			this.data.ChangeAttr(AttrType.Hp, OpType.Add, 1f - base.hp);
		}
		if (attacker != null && base.hp <= 0f && attacker.unique_id == this.unique_id && !base.MirrorState)
		{
			this.data.ChangeAttr(AttrType.Hp, OpType.Add, 1f);
		}
		this.data.OnWound(attacker, damage);
		this.statistics.OnWound(attacker, damage);
		this.surface.OnWound(attacker, damage);
		if (this.aiManager != null)
		{
			this.aiManager.OnWound(attacker, damage);
		}
		this.DispatchWoundEvent(damage);
		NewbieManager.Instance.OnUnitWounded(this, attacker);
		if (attacker != null && (attacker.isHero || attacker.isPlayer) && attacker.unique_id != this.unique_id)
		{
			base.LastHurtHero = attacker;
			if (this.m_assistantDict.ContainsKey(attacker))
			{
				this.m_assistantDict[attacker] = Time.time;
			}
			else
			{
				this.m_assistantDict.Add(attacker, Time.time);
			}
		}
	}

	public override void TryDeath(Units attacker)
	{
		if (!base.isLive)
		{
			return;
		}
		if (base.hp > 0f)
		{
			return;
		}
		string text = string.Empty;
		if (attacker != null)
		{
			text = attacker.name;
		}
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			return;
		}
		if (base.CanRebirth.IsInState)
		{
			this.PreDeath(attacker);
			base.isLive = false;
			this.DispatchRebirthEvent();
		}
		else
		{
			if (!base.CheckDeathCondition(attacker))
			{
				return;
			}
			ActionManager.BecomeDead(attacker, this);
		}
	}

	public override void PreDeath(Units attacker)
	{
		this.m_CoroutineManager.StopAllCoroutine();
		base.EnableAction(false);
		this.data.OnDeath(attacker);
		this.surface.OnDeath(attacker);
		this.animController.OnDeath(attacker);
		if (this.aiManager != null)
		{
			this.aiManager.OnDeath(attacker);
		}
		if (this.moveController != null)
		{
			this.moveController.OnStop();
		}
		if (this.atkController != null)
		{
			this.atkController.OnStop();
		}
		Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitPrognosisDeath, this, null, null);
		if (attacker != null && (attacker.isHero || attacker.isPlayer) && attacker.unique_id != this.unique_id)
		{
			base.LastHurtHero = attacker;
		}
		if (base.isLocalUnit)
		{
			if (GlobalSettings.FogMode >= 2)
			{
				FOWSystem.CreateStaticTimeRevealer(base.transform.position, 1f, 3f);
			}
			else if (GlobalSettings.FogMode == 1)
			{
				FogMgr.Instance.AddFogItem(base.transform.position, 1f, 3f);
			}
		}
	}

	public override void RealDeath(Units attacker)
	{
		base.mCoroutineManager.StopAllCoroutine();
		if (!this.isMonster)
		{
			this.skillManager.OnDeath(attacker);
		}
		this.buffManager.OnDeath(attacker);
		this.highEffManager.OnDeath(attacker);
		this.effectManager.OnDeath(attacker);
		this.statistics.OnDeath(attacker);
		this.DeathIncome(attacker);
		this.DispatchDeadEvent(attacker);
		if (Singleton<UnitVisibilityManager>.Instance != null)
		{
			Singleton<UnitVisibilityManager>.Instance.ClearUnitGrassInfo(this.unique_id, (TeamType)base.teamType);
		}
		if (this.isPlayer)
		{
			Singleton<SkillView>.Instance.ClearSelectState();
		}
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			Units obserserUnit = PvpObserveMgr.GetObserserUnit();
			if (obserserUnit != null && obserserUnit.unique_id == this.unique_id)
			{
				base.playVoice("onDie");
			}
			if (obserserUnit != null && attacker && attacker.unique_id == obserserUnit.unique_id)
			{
				attacker.playVoice("onSkillAttack");
			}
		}
		else
		{
			if (this.isPlayer)
			{
				base.playVoice("onDie");
			}
			if (attacker && attacker.isPlayer)
			{
				attacker.playVoice("onSkillAttack");
			}
		}
		if (this == PlayerControlMgr.Instance.GetSelectedTarget())
		{
			PlayerControlMgr.Instance.TryUpdateTargetIcon();
		}
	}

	public override void Rebirth()
	{
		base.Rebirth();
		this.DispatchRebirthEvent();
	}

	protected virtual void DeathIncome(Units attacker)
	{
	}

	protected override void DispatchDeadEvent(Units attacker)
	{
		if (base.MirrorState)
		{
			return;
		}
		if (this.isHero && attacker != null && (attacker.isHero || attacker.isPlayer))
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitKillAndAssist, attacker, null, null);
		}
		MobaMessageManager.DispatchMsg(MobaMessageManager.GetMessage((ClientMsg)25056, new ParamUnitDead(attacker, this), 0f));
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.UntiDead);
		Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitKillTarget, attacker, this, null);
		Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitDeath, this, attacker, null);
		if (this.isHero)
		{
			if (base.teamType == 0)
			{
				Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.LMHeroDead);
			}
			else if (base.teamType == 1)
			{
				Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.BLHeroDead);
			}
		}
		base.DispatchDeadEvent(attacker);
	}

	protected override void DispatchRebirthEvent()
	{
		Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitRebirth, this, null, null);
		base.DispatchRebirthEvent();
	}
}
