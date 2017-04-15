using Com.Game.Module;
using MobaHeros.AI;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class AIManager : UnitComponent
{
	private struct AttackTime
	{
		public Units attacker;

		public float time;

		public AttackTime(Units attacker, float time)
		{
			this.attacker = attacker;
			this.time = time;
		}
	}

	public enum UnitEvents
	{
		Spawned,
		Death
	}

	private const int AI_TYPE_SOLIDER = 1;

	private const int AI_TYPE_TOWER = 2;

	private const int AI_TYPE_MONSTER = 3;

	private const int AI_TYPE_HOME = 4;

	private const int AI_TYPE_ITEM = 5;

	private const int AI_TYPE_BOSS = 7;

	private const int AI_TYPE_ASSIST_BOSS = 9;

	private const int AI_TYPE_VIEWER = 8;

	private bool isActive;

	private SensoryMemory m_SensoryMemory;

	private TargetingSystem m_TargetingSystem;

	private List<AIManager.AttackTime> m_attackerTimeMap = new List<AIManager.AttackTime>();

	private float m_fMaxAttackerKeepTime = 4f;

	private List<string> skill_pority_queue;

	private Units skillHitedTarget;

	private Units attackedYouTarget;

	private Units skillHitedYouTarget;

	[SerializeField]
	private Units m_lastKilledTarget;

	private Task preTask;

	private GameObject aiObject
	{
		get
		{
			if (this.self != null)
			{
				return this.self.mAIObject;
			}
			return null;
		}
	}

	private GameObject memObject
	{
		get
		{
			if (this.self != null)
			{
				return this.self.mMemory;
			}
			return null;
		}
	}

	[SerializeField]
	public Units CurAttackTarget
	{
		get
		{
			if (!this.self.ChaoFeng.IsInState && this.self.isPlayer)
			{
				if (this.m_TargetingSystem == null)
				{
					return null;
				}
				Units selectTarget = this.m_TargetingSystem.GetSelectTarget();
				if (selectTarget != null && selectTarget.isLive)
				{
					return selectTarget;
				}
			}
			if (this.m_TargetingSystem != null)
			{
				return this.m_TargetingSystem.GetAttackTarget();
			}
			return null;
		}
	}

	public Units CurTreeAtkTarget
	{
		get
		{
			if (this.m_TargetingSystem != null)
			{
				return this.m_TargetingSystem.GetTreeAtkTarget();
			}
			return null;
		}
	}

	public Units CurAttackYouTarget
	{
		get
		{
			return null;
		}
	}

	public Units GuardTarget
	{
		get
		{
			return this.m_TargetingSystem.GetGuardTarget();
		}
	}

	public Units LastKilledTarget
	{
		get
		{
			return this.m_lastKilledTarget;
		}
		set
		{
			this.m_lastKilledTarget = value;
		}
	}

	public AIManager()
	{
	}

	public AIManager(Units self) : base(self)
	{
	}

	[DebuggerHidden]
	public IEnumerable<Units> GetRecentAttacker(float secondsBefore)
	{
		AIManager.<GetRecentAttacker>c__Iterator23 <GetRecentAttacker>c__Iterator = new AIManager.<GetRecentAttacker>c__Iterator23();
		<GetRecentAttacker>c__Iterator.secondsBefore = secondsBefore;
		<GetRecentAttacker>c__Iterator.<$>secondsBefore = secondsBefore;
		<GetRecentAttacker>c__Iterator.<>f__this = this;
		AIManager.<GetRecentAttacker>c__Iterator23 expr_1C = <GetRecentAttacker>c__Iterator;
		expr_1C.$PC = -2;
		return expr_1C;
	}

	public void EnableSearchTarget(bool b)
	{
		this.m_TargetingSystem.IsSearchTarget = b;
	}

	public void SetSkillHitedTarget(Units target)
	{
		this.skillHitedTarget = target;
	}

	public Units GetSkillHitedTarget()
	{
		return this.skillHitedTarget;
	}

	public void SetAttackedYouTarget(Units target)
	{
		this.attackedYouTarget = target;
	}

	public Units GetAttackedYouTarget()
	{
		return this.attackedYouTarget;
	}

	public void SetSkillHitedYouTarget(Units target)
	{
		this.skillHitedYouTarget = target;
	}

	public Units GetSkillHitedYouTarget()
	{
		return this.skillHitedYouTarget;
	}

	public SensoryMemory GetSensoryMemory()
	{
		return this.m_SensoryMemory;
	}

	public TargetingSystem GetTargetingSystem()
	{
		return this.m_TargetingSystem;
	}

	public override void OnInit()
	{
		if (Singleton<PvpManager>.Instance.IsInPvp && !this.self.isHero)
		{
			this.isActive = false;
			if (this.self.mMemory)
			{
				UnityEngine.Debug.LogError(this.self.name + " has mMemory");
				UnityEngine.Object.Destroy(this.self.mMemory);
			}
			if (this.self.mVechileCollider)
			{
				UnityEngine.Object.Destroy(this.self.mVechileCollider);
			}
			if (this.self.mObstacleObj)
			{
				UnityEngine.Object.Destroy(this.self.mObstacleObj);
			}
			if (this.self.mObstacleCollider)
			{
				UnityEngine.Object.Destroy(this.self.mObstacleCollider);
			}
			if (this.self.mAIObject)
			{
				UnityEngine.Object.Destroy(this.self.mAIObject);
			}
			return;
		}
		this.isActive = true;
		this.StartAI();
	}

	public void ChangeTargetingSystem(Units mainHero)
	{
		this.self.SetMirrorState(true);
		this.OnInit();
		HeroMirrorTargetingSystem heroMirrorTargetingSystem = this.m_TargetingSystem as HeroMirrorTargetingSystem;
		heroMirrorTargetingSystem.SetFollowTarget(mainHero);
		this.self.unitControlType = UnitControlType.Free;
	}

	public override void OnStart()
	{
		if (this.m_SensoryMemory != null)
		{
			this.m_SensoryMemory.UpdateBotsWithSpawned();
		}
		this.BroadcastEventToMap(Relation.All, TargetTag.All, AIManager.UnitEvents.Spawned);
	}

	public override void OnStop()
	{
		this.isActive = false;
		if (this.m_SensoryMemory != null)
		{
			this.m_SensoryMemory.ClearMemory();
		}
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.ClearTargets();
		}
		this.m_attackerTimeMap.Clear();
		this.m_lastKilledTarget = null;
	}

	public override void OnExit()
	{
		this.isActive = false;
		if (this.m_SensoryMemory != null)
		{
			this.m_SensoryMemory.ClearMemory();
			this.m_SensoryMemory = null;
		}
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.ClearTargets();
			this.m_TargetingSystem = null;
		}
		this.m_attackerTimeMap.Clear();
		this.m_lastKilledTarget = null;
	}

	public override void OnUpdate(float delta)
	{
		if (GlobalSettings.TestDisableAI)
		{
			return;
		}
		if (!this.isActive)
		{
			return;
		}
		if (this.self.isPlayer)
		{
			PlayerControlMgr.Instance.SetSelectedTarget(this.CurAttackTarget);
		}
		this.UpdateAIState();
	}

	public override void OnWound(Units attacker, float damage)
	{
		if (attacker != null)
		{
			this.m_attackerTimeMap.RemoveAll((AIManager.AttackTime x) => !x.attacker || x.attacker == attacker || Time.time - x.time > this.m_fMaxAttackerKeepTime);
			this.m_attackerTimeMap.Add(new AIManager.AttackTime(attacker, Time.time));
		}
	}

	public override void OnDeath(Units attacker)
	{
		if (this.m_SensoryMemory != null)
		{
			this.m_SensoryMemory.ClearMemory();
		}
	}

	private SensoryMemory CreateSensorMemory()
	{
		return new SensoryMemory(this.self, this.memObject, 10.0);
	}

	public void SpecialEffectBegin(SpecialSkillEff eff)
	{
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.OnSpecialSKillEffBegin(eff);
		}
		if (this.self != null)
		{
			this.self.StopMove();
		}
	}

	public void SpecialEffectEnd(SpecialSkillEff eff)
	{
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.OnSpecialSkillEffEnd(eff);
		}
	}

	public void DefendTower(Units attacker, Units tower)
	{
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.DefendTower(attacker, tower);
		}
	}

	private string PopSkill()
	{
		string text = this.skill_pority_queue.First<string>();
		this.skill_pority_queue.Remove(text);
		this.skill_pority_queue.Add(text);
		return text;
	}

	private string GetFrontSkill()
	{
		return this.skill_pority_queue.First<string>();
	}

	public void SetSelectTarget(Units target)
	{
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.SetSelectTarget(target);
		}
	}

	public void SetAttackTarget(Units target)
	{
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.SetAttackTarget(target);
		}
	}

	public void SetGuardTarget(Units target)
	{
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.SetGuardTarget(target);
		}
	}

	public void SetTauntTarget(Units target)
	{
		if (this.m_TargetingSystem != null)
		{
			if (this.self != null)
			{
				this.self.StopMove();
				this.self.SetCanAIControl(true);
			}
			this.m_TargetingSystem.SetTauntTarget(target);
		}
	}

	public void SetAttackYouTarget(Units target)
	{
		if (this.m_TargetingSystem != null)
		{
			this.m_TargetingSystem.SetAttackYouTarget(target);
		}
	}

	public Units GetAttackTarget()
	{
		return this.CurAttackTarget;
	}

	public Units GetAttackYouTarget()
	{
		return this.CurAttackYouTarget;
	}

	public int GetTargetHatredValue(Units target)
	{
		if (this.m_SensoryMemory != null)
		{
			return this.m_SensoryMemory.GetHatredValue(target);
		}
		return 0;
	}

	public int GetTargetPorityValue(Units target)
	{
		if (this.m_SensoryMemory != null)
		{
			return this.m_SensoryMemory.GetTargetPorityValue(target);
		}
		return 0;
	}

	public void BroadcastEventToMap(Relation relation, TargetTag tagType, AIManager.UnitEvents unit_event)
	{
		Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
		Dictionary<int, Units>.Enumerator enumerator = allMapUnits.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, Units> current = enumerator.Current;
			Units value = current.Value;
			if (!(value == null) && value.unique_id != this.self.unique_id)
			{
				if (TagManager.CheckTag(value, tagType))
				{
					value.ReceiveEvent(this.self, unit_event);
				}
			}
		}
	}

	public void BoradcastEventToArround(Relation relation, TargetTag tag, AIManager.UnitEvents unit_event)
	{
		if (this.m_SensoryMemory != null)
		{
			List<Units> listOfRecentlySensedOpponents = this.m_SensoryMemory.GetListOfRecentlySensedOpponents(relation, tag, true, false, SortType.None, FindType.None, null);
			if (listOfRecentlySensedOpponents != null)
			{
				for (int i = 0; i < listOfRecentlySensedOpponents.Count; i++)
				{
					if (listOfRecentlySensedOpponents[i] != null && listOfRecentlySensedOpponents[i] != this.self)
					{
						listOfRecentlySensedOpponents[i].ReceiveEvent(this.self, unit_event);
					}
				}
			}
		}
	}

	public void ReceiveEvent(Units sender, AIManager.UnitEvents uint_events)
	{
		if (sender != null)
		{
			if (uint_events != AIManager.UnitEvents.Spawned)
			{
				if (uint_events == AIManager.UnitEvents.Death)
				{
					if (this.m_SensoryMemory != null)
					{
						this.m_SensoryMemory.UpdateBotWithDeath(sender);
					}
				}
			}
			else if (this.m_SensoryMemory != null)
			{
				this.m_SensoryMemory.UpdateBotWithSpawned(sender);
			}
		}
	}

	public void UpdateBehavior(List<Units> targets)
	{
		if (GlobalSettings.TestDisableAI)
		{
			return;
		}
		if (!this.isActive)
		{
			return;
		}
		if (this.self.CanAIControl)
		{
			if (this.m_SensoryMemory != null)
			{
				this.m_SensoryMemory.UpdateMemory(targets);
			}
			if (this.m_TargetingSystem != null)
			{
				this.m_TargetingSystem.Update();
			}
		}
	}

	public void StopAI()
	{
	}

	public void StartAI()
	{
	}

	public void UpdateAIState()
	{
		if (this.self.CanAIControl)
		{
			this.StartAI();
		}
		else
		{
			this.StopAI();
		}
	}

	public void ChangeTeamMessage()
	{
		if (this.m_SensoryMemory != null)
		{
			this.m_SensoryMemory.ClearMemory();
		}
	}
}
