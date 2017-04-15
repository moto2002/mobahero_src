using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using Common;
using MobaFrame.SkillAction;
using MobaHeros.AI;
using MobaHeros.Pvp;
using MobaHeros.Spawners;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Monster : MovingEntity
{
	private enum EMonsterSleepStatus
	{
		DefaultVal,
		NotSleep,
		Sleep,
		WakeUp
	}

	private bool _fromNeutralMonster;

	private string _creepId;

	private int _monsterTeamId;

	public Vector3 targetPosFromNet;

	public bool isDrawGizmos;

	private float fIdleTimer;

	private EMonsterCreepAiStatus _monsterCreepAiStatus;

	private eventPlayerSound m_eventPlayerSound;

	private string musicid = string.Empty;

	private SysMonsterMainVo configMosterMainVo;

	private AnimPlayer ap;

	private bool neutralRelive;

	public bool FromNeutralMonster
	{
		get
		{
			return this._fromNeutralMonster;
		}
	}

	public override void SetOrigin(bool fromNeutralMonster, string id, int monsterTeamId)
	{
		this._fromNeutralMonster = fromNeutralMonster;
		this._creepId = id;
		if (BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(this._creepId) != null)
		{
			this._monsterTeamId = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(this._creepId).monstergroup_id;
		}
	}

	public override string GetBattleMonsterCreepId()
	{
		return this._creepId;
	}

	public override int GetBattleMonsterTeamId()
	{
		return this._monsterTeamId;
	}

	public override void TryAddBirthEffect()
	{
		if (!StringUtils.CheckValid(this._creepId))
		{
			return;
		}
		SysBattleMonsterCreepVo dataById = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(this._creepId);
		if (dataById == null)
		{
			return;
		}
		if (!StringUtils.CheckValid(dataById.perform_id))
		{
			return;
		}
		string[] array = dataById.perform_id.Split(new char[]
		{
			','
		});
		if (array.Length <= 0)
		{
			return;
		}
		string text = string.Empty;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (StringUtils.CheckValid(array[i]))
			{
				string[] array2 = array[i].Split(new char[]
				{
					'|'
				});
				if (array2.Length == 2)
				{
					if (int.TryParse(array2[0], out num))
					{
						if (num == 4)
						{
							text = array2[1];
							break;
						}
					}
				}
			}
		}
		if (!StringUtils.CheckValid(text))
		{
			return;
		}
		base.StartEffect(text);
	}

	protected override void OnInit(bool isRebirth = false)
	{
		this.configMosterMainVo = BaseDataMgr.instance.GetMonsterMainData(this.npc_id);
		this.musicid = this.configMosterMainVo.music_id;
		AudioMgr.loadSoundBank_Skill(this.musicid, true, 0);
		if (this.configMosterMainVo == null)
		{
			ClientLogger.Warn("Ignore scaling, cannot found SysMonsterMainVo: " + this.npc_id);
		}
		else
		{
			this.DoScale(this.configMosterMainVo.scale);
		}
		base.OnInit(false);
		if (!base.IsMonsterCreep())
		{
		}
		if (base.UnitType == UnitType.EyeUnit || base.IsMonsterCreep() || base.UnitType == UnitType.Soldier)
		{
			this.m_nServerVisibleState = 2;
		}
	}

	private void DoScale(float scale)
	{
		if ((double)Math.Abs(base.transform.localScale.x - scale) > 0.001)
		{
			base.transform.localScale = Vector3.one * scale;
		}
	}

	protected override void OnCreate()
	{
		this.ap = base.GetComponentInChildren<AnimPlayer>();
		this.data = base.AddUnitComponent<MonsterDataManager>(!Singleton<PvpManager>.Instance.IsInPvp);
		this.atkController = base.AddUnitComponent<MonsterAttackController>();
		base.OnCreate();
		base.ChangeLayer("Monster");
		if (this.m_model != null)
		{
			eventPlayerSound eventPlayerSound = this.m_model.gameObject.GetComponent<eventPlayerSound>();
			if (eventPlayerSound == null)
			{
				eventPlayerSound = this.m_model.gameObject.AddComponent<eventPlayerSound>();
			}
			eventPlayerSound.units = this;
			this.m_eventPlayerSound = eventPlayerSound;
		}
	}

	protected override void DeathIncome(Units attacker)
	{
		UtilManager.Instance.AddDeathGold(attacker, this);
		UtilManager.Instance.AddExp(attacker, this);
		UtilManager.Instance.UpdateKillState(attacker, this);
		CreepHelper.AddBuff(this._creepId, attacker);
	}

	public override void Wound(Units attacker, float damage)
	{
		if (damage > 0f)
		{
			return;
		}
		if (this.isVisible && this.isVisibleInCamera)
		{
			this.surface.ShowRimLight();
		}
		if (base.IsMonsterCreep() && this.animController != null)
		{
			this.animController.OnWound(attacker, damage);
		}
		base.Wound(attacker, damage);
	}

	protected override void OnUpdate(float delta)
	{
		base.UpdateCameraVisible();
		base.OnUpdate(delta);
		if (base.mHpBar != null && this.m_fLiveTime > 0f && this.m_fLeftTime >= 0f)
		{
			this.m_fLeftTime -= delta;
			base.mHpBar.SetLiveTime(this.m_fLeftTime / this.m_fLiveTime);
		}
		if (this.npc_id == "54321")
		{
			this.UpdateIdle(delta);
		}
	}

	private void UpdateIdle(float delta)
	{
		if (this.moveController != null && !this.moveController.isMoving)
		{
			this.fIdleTimer += delta;
		}
		else
		{
			this.fIdleTimer = 0f;
		}
		if (this.fIdleTimer > 1.5f)
		{
			this.fIdleTimer = 0f;
			if (this.moveController != null && this.animController != null)
			{
				this.animController.RandomIdle();
			}
		}
	}

	protected override void OnVisibleInCamera()
	{
		if (this.moveController.isMoving)
		{
			this.animController.PlayAnim(AnimationType.Move, true, 0, true, false);
		}
	}

	private void UpdatePetInvisibleState()
	{
		if (base.ParentUnit != null && base.UnitType == UnitType.Pet)
		{
			if (base.ParentUnit.m_nVisibleState >= 1)
			{
				base.Invisible.SetState(1);
			}
			else
			{
				base.Invisible.SetState(0);
			}
		}
	}

	public override void TryDeath(Units attacker)
	{
		if (base.IsMonsterCreep())
		{
			this.MonsterCreepTryDeath(attacker);
			return;
		}
		base.TryDeath(attacker);
	}

	private void MonsterCreepTryDeath(Units attacker)
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
			attacker.StopAttack();
		}
		if (base.CanRebirth.IsInState)
		{
			base.isLive = false;
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				ActionManager.BecomeFakeDead(attacker, this);
			}
			else
			{
				this.DispatchRebirthEvent();
			}
		}
		else
		{
			if (!base.CheckDeathCondition(attacker))
			{
				return;
			}
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				ActionManager.BecomeFakeDead(attacker, this);
			}
			else
			{
				ActionManager.BecomeDead(attacker, this);
			}
		}
	}

	public override void PseudoDeath(int inOldGroupType, int inNewGroupType, float inHpVal, float inMpVal, string inNpcId, string inBattleMonsterCreepId, Units inAttacker)
	{
		base.ChangeTeam((TeamType)inNewGroupType);
		if (StringUtils.CheckValid(inNpcId))
		{
			this.npc_id = inNpcId;
		}
		this.skillManager.isFirstInited = false;
		base.UnitInit(false);
		this.unitCollider.OriginalTeamType = inOldGroupType;
		MapManager.Instance.OnMapUnitSwitchCamp(this, inOldGroupType, inNewGroupType);
		base.UnitStart();
		this.SetBloodBar();
		base.SetHp(inHpVal);
		base.SetMaxHp(inHpVal);
		base.MarkAsTarget(false);
	}

	private void SetBloodBar()
	{
		if (this.surface != null)
		{
			this.surface.SetBloodBar(base.isMyTeam);
		}
	}

	private bool NeutralMonsterDeath(Units attacker)
	{
		if (base.hp > 0f)
		{
			return false;
		}
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			return false;
		}
		if (this.neutralRelive)
		{
			return false;
		}
		if (base.teamType != 2)
		{
			return false;
		}
		SysBattleMonsterCreepVo reliveCreepVo = CreepSpawner.GetReliveCreepVo(this.unique_id);
		if (reliveCreepVo == null)
		{
			return false;
		}
		if (StringUtils.CheckValid(reliveCreepVo.summons2))
		{
			SysBattleMonsterCreepVo dataById = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(reliveCreepVo.summons2);
			string[] stringValue = StringUtils.GetStringValue(dataById.monsters, '|');
			base.ChangeTeam((TeamType)attacker.teamType);
			if (Singleton<CreepSpawner>.Instance != null)
			{
				Singleton<CreepSpawner>.Instance.AddNPCId(stringValue[0]);
			}
			this.neutralRelive = true;
			MapManager.Instance.ReliveNeutralMonster(this, stringValue[0], attacker.teamType);
			string fxName = CreepHelper.GetFxName(this._creepId, CreepFxType.creep_under_control);
			if (fxName != null)
			{
				ActionManager.PlayEffect(fxName, this, null, null, true, string.Empty, null);
			}
			return true;
		}
		return false;
	}

	public override void RealDeath(Units attacker)
	{
		CreepHelper.TryShowCreepUIMessageInfo(attacker, this, this.GetBattleMonsterCreepId(), base.teamType);
		base.TryHideNewbieHintObj();
		base.RealDeath(attacker);
	}

	public void Sleep()
	{
		base.Sleeping.Add();
		base.mHpBar.setActive(false);
		if (this.aiManager != null)
		{
			this.aiManager.StopAI();
		}
		if (this.animController != null)
		{
			CoroutineManager coroutineManager = new CoroutineManager();
			coroutineManager.StartCoroutine(this.YieldPlaySleep(), true);
		}
	}

	protected override void OnStart()
	{
		base.OnStart();
		if (this.ap != null && this.isVisibleInCamera)
		{
			this.ap.PlayAnimate("breath", WrapMode.Loop);
		}
	}

	[DebuggerHidden]
	private IEnumerator YieldPlaySleep()
	{
		Monster.<YieldPlaySleep>c__Iterator1C <YieldPlaySleep>c__Iterator1C = new Monster.<YieldPlaySleep>c__Iterator1C();
		<YieldPlaySleep>c__Iterator1C.<>f__this = this;
		return <YieldPlaySleep>c__Iterator1C;
	}

	[DebuggerHidden]
	private IEnumerator YieldPlayWakeup(bool direct)
	{
		Monster.<YieldPlayWakeup>c__Iterator1D <YieldPlayWakeup>c__Iterator1D = new Monster.<YieldPlayWakeup>c__Iterator1D();
		<YieldPlayWakeup>c__Iterator1D.<>f__this = this;
		return <YieldPlayWakeup>c__Iterator1D;
	}

	public void Wakeup(bool direct = false)
	{
		if (!base.IsMonsterCreep())
		{
			return;
		}
		CoroutineManager coroutineManager = new CoroutineManager();
		coroutineManager.StartCoroutine(this.YieldPlayWakeup(direct), true);
	}

	public void Appear()
	{
		if (this.animController != null)
		{
			this.animController.PlayAnim(AnimationType.Sleep, false, 1, true, false);
		}
		CreepHelper.ShowCreepWakePrompt(this);
	}

	public override bool IsMonsterCreepAiStatus(EMonsterCreepAiStatus inMonsterCreepAiStatus)
	{
		return this._monsterCreepAiStatus == inMonsterCreepAiStatus;
	}

	public override void SetMonsterCreepAiStatus(EMonsterCreepAiStatus inMonsterCreepAiStatus)
	{
		this._monsterCreepAiStatus = inMonsterCreepAiStatus;
	}

	protected override bool IsConfigedAttackable()
	{
		return this.configMosterMainVo == null || this.configMosterMainVo.is_attackable > 0;
	}

	protected override bool IsConfigedSelectable()
	{
		return this.configMosterMainVo == null || this.configMosterMainVo.is_selectable > 0;
	}
}
