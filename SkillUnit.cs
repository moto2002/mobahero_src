using Com.Game.Module;
using MobaHeros;
using MobaHeros.Pvp;
using System;
using UnityEngine;

public class SkillUnit : Units
{
	protected SkillUnitDataManager unit_data;

	public float existTime;

	public int itemType;

	private float curTime;

	private bool noDeath = true;

	protected override void OnCreate()
	{
		this.data = base.AddUnitComponent<SkillUnitDataManager>();
		this.unitCollider = base.AddUnitComponent<UnitCollider>();
		this.dataChange = base.AddUnitComponent<DataChangeManager>();
		this.effectManager = base.AddUnitComponent<EffectManager>();
		this.skillManager = base.AddUnitComponent<ItemSkillManager>();
		this.highEffManager = base.AddUnitComponent<HighEffManager>();
		this.atkController = base.AddUnitComponent<ItemAttackController>();
		this.surface = base.AddUnitComponent<SurfaceManager>();
		base.ImmunityManager = base.AddUnitComponent<ImmunityManager>();
		if (this.IsSyncPosition)
		{
			this.moveController = base.AddUnitComponent<MoveController>();
		}
		base.ChangeLayer("SkillUnit");
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			this.buffManager = base.AddUnitComponent<PVP_BuffManager>();
		}
		else
		{
			this.buffManager = base.AddUnitComponent<BuffManager>();
		}
		if (base.gameObject.GetComponent<CharacterEffect>() == null)
		{
			base.gameObject.AddComponent<CharacterEffect>();
		}
		if (base.mCharacterEffect == null)
		{
			base.mCharacterEffect = base.gameObject.GetComponent<CharacterEffect>();
		}
	}

	protected override void OnInit(bool isRebirth = false)
	{
		base.OnInit(false);
		this.unit_data = (this.data as SkillUnitDataManager);
		this.existTime = this.unit_data.data.config.exist_time;
		this.itemType = this.unit_data.data.config.item_type;
		if (this.noDeath)
		{
			base.ChangeAttr(AttrType.Hp, OpType.Add, 99999f);
			base.ChangeAttr(AttrType.Mp, OpType.Add, 99999f);
		}
		this.curTime = 0f;
		this.m_Radius = 0f;
		this.m_SelectRadius = 0f;
		this.m_nServerVisibleState = 2;
	}

	protected override void OnStart()
	{
		base.OnStart();
	}

	protected override void OnUpdate(float delta)
	{
		base.OnUpdate(delta);
		if (this.existTime > 0f && !Singleton<PvpManager>.Instance.IsInPvp)
		{
			this.curTime += delta;
			if (this.curTime >= this.existTime)
			{
				this.curTime = 0f;
				this.RemoveSelf(0f);
			}
		}
	}

	protected override void OnStop()
	{
		base.OnStop();
	}

	public override void TryDeath(Units attacker)
	{
		if (!base.isLive)
		{
			return;
		}
		if (base.hp > 0f || this.noDeath)
		{
			return;
		}
		base.isLive = false;
	}

	protected override void OnEnd()
	{
		base.OnEnd();
		this.RemoveSelf(0f);
	}

	protected override void OnExit()
	{
		base.isLive = false;
		base.OnExit();
		this.RemoveSelf(0f);
	}

	public override void RemoveSelf(float delay = 0f)
	{
		base.RemoveSelf(delay);
	}

	protected virtual void ShowEffect()
	{
		if (this.unit_data.data.config.item_type == 1)
		{
			return;
		}
		if (MapManager.Instance != null && PlayerControlMgr.Instance.GetPlayer() != null && !TeamManager.CheckTeam(base.gameObject, PlayerControlMgr.Instance.GetPlayer().teamType))
		{
			this.EnableAllRenders(false);
		}
	}

	public override void EnableAllRenders(bool b)
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = b;
		}
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitTriggerEnter, this, null, null);
	}

	public virtual void OnTriggerExit(Collider other)
	{
	}
}
