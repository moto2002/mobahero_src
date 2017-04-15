using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StrategyManager
{
	public enum ControlState
	{
		ManualState = 1,
		AutoState,
		None
	}

	private static StrategyManager m_instance;

	private Strategy _lmStrategy;

	private Strategy _blStrategy;

	private readonly Dictionary<int, bool> _homeRecovery = new Dictionary<int, bool>();

	private readonly Dictionary<int, Units> _heroAvoidence = new Dictionary<int, Units>();

	private VTrigger _towerDefendTrigger;

	private VTrigger _heroAvoidenceTrigger;

	private Task _lmHeroAvoidTask;

	private Task _blHeroAvoidTask;

	private bool[] _inputQueue;

	private int _QueueLength = 2;

	private StrategyManager.ControlState _curState = StrategyManager.ControlState.None;

	private float CONST_TIME_TO_AUTO = 99999f;

	private float _time2Auto = 99999f;

	private Task _curTimeTask;

	private bool _ifPlayerStopCure;

	private Task preTask;

	private List<string> table = new List<string>(new string[]
	{
		"301",
		"302",
		"303",
		"304",
		"305",
		"306",
		"307",
		"308",
		"309",
		"310",
		"311",
		"312"
	});

	private SysBattleRefreshVo m_lm_brVo;

	private int m_lm_locationId;

	private SysBattleRefreshVo m_bl_brVo;

	private int m_bl_locationId;

	private Dictionary<TeamType, Units> _buffHeros = new Dictionary<TeamType, Units>();

	public static StrategyManager Instance
	{
		get
		{
			if (StrategyManager.m_instance == null)
			{
				StrategyManager.m_instance = new StrategyManager();
			}
			return StrategyManager.m_instance;
		}
	}

	public StrategyManager.ControlState CurState
	{
		get
		{
			return this._curState;
		}
	}

	public float Time2Auto
	{
		get
		{
			return this._time2Auto;
		}
	}

	public bool IfPlayerStopCure
	{
		get
		{
			return this._ifPlayerStopCure;
		}
	}

	public Strategy LMStrategy
	{
		get
		{
			return this._lmStrategy;
		}
	}

	public Strategy BlStrategy
	{
		get
		{
			return this._blStrategy;
		}
	}

	private SysBattleRefreshVo lm_brVo
	{
		get
		{
			if (this.m_lm_brVo == null)
			{
				this.m_lm_brVo = BaseDataMgr.instance.GetDataById<SysBattleRefreshVo>("301");
				this.m_lm_locationId = StringUtils.GetStringToInt(this.m_lm_brVo.locationid, ',')[0];
			}
			return this.m_lm_brVo;
		}
	}

	private SysBattleRefreshVo bl_brVo
	{
		get
		{
			if (this.m_bl_brVo == null)
			{
				this.m_bl_brVo = BaseDataMgr.instance.GetDataById<SysBattleRefreshVo>("302");
				this.m_bl_locationId = StringUtils.GetStringToInt(this.m_bl_brVo.locationid, ',')[0];
			}
			return this.m_bl_brVo;
		}
	}

	private StrategyManager()
	{
		this._lmStrategy = new Strategy(TeamType.LM);
		this._blStrategy = new Strategy(TeamType.BL);
		this.RegisterListeners();
		this.InitAvoidence();
		this.InitInputState();
	}

	private void RegisterListeners()
	{
		this._towerDefendTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDefendTower, null, new TriggerAction(this.DefendTower), -1, "Home");
		this._heroAvoidenceTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.HeroAvoidence, null, new TriggerAction(this.HeroAvoidence), -1, "Hero");
	}

	private void UnRegisterListerners()
	{
		TriggerManager.DestroyTrigger(this._towerDefendTrigger);
		TriggerManager.DestroyTrigger(this._heroAvoidenceTrigger);
		this._towerDefendTrigger = null;
		this._heroAvoidenceTrigger = null;
	}

	private void InitInputState()
	{
		this._inputQueue = new bool[this._QueueLength];
		this.ResetInputState();
		this.SetControlState(StrategyManager.ControlState.ManualState, -1f);
	}

	private void ResetInputState()
	{
		for (int i = 0; i < this._QueueLength; i++)
		{
			this._inputQueue[i] = true;
		}
	}

	public void SetControlState(StrategyManager.ControlState state, float time2Change = -1f)
	{
		if (this._curTimeTask != null)
		{
			this._curTimeTask.Stop();
		}
		if (state == StrategyManager.ControlState.ManualState)
		{
			if (time2Change != -1f)
			{
				this.CONST_TIME_TO_AUTO = time2Change;
			}
			this._time2Auto = this.CONST_TIME_TO_AUTO;
			CoroutineManager coroutineManager = new CoroutineManager();
			this._curTimeTask = coroutineManager.StartCoroutine(this.ControlTimeTick(), true);
		}
		else if (state == StrategyManager.ControlState.AutoState)
		{
			this._time2Auto = 0f;
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player != null)
			{
				player.SetCanAIControl(true);
			}
		}
		this._curState = state;
	}

	[DebuggerHidden]
	private IEnumerator ControlTimeTick()
	{
		StrategyManager.<ControlTimeTick>c__Iterator24 <ControlTimeTick>c__Iterator = new StrategyManager.<ControlTimeTick>c__Iterator24();
		<ControlTimeTick>c__Iterator.<>f__this = this;
		return <ControlTimeTick>c__Iterator;
	}

	public static bool IsInSpecialState(Units target)
	{
		return !(target == null) && (target.MeiHuo.IsInState || target.ChaoFeng.IsInState);
	}

	public void GameStartState()
	{
		UtilManager.Instance.Init();
		if (LevelManager.CurBattleType == 6)
		{
			this.SetControlState(StrategyManager.ControlState.AutoState, -1f);
		}
		else
		{
			this.SetControlState(StrategyManager.ControlState.ManualState, -1f);
		}
	}

	public void UpdateInputState(bool hasTarget)
	{
		Units player = PlayerControlMgr.Instance.GetPlayer();
		if (!hasTarget)
		{
			player.SetSelectTarget(null);
		}
		this.SetControlState(StrategyManager.ControlState.ManualState, -1f);
	}

	public void UpdateControlState()
	{
		this.SetControlState(StrategyManager.ControlState.ManualState, -1f);
	}

	public bool IsAuto()
	{
		return this._time2Auto <= 0f;
	}

	public void ChangePlayerCureState()
	{
		if (!this.IsPlayerNearHome())
		{
			return;
		}
		if (this.preTask != null)
		{
			this.preTask.Stop();
			this.preTask = null;
		}
		Task task = new Task(this.DelayChange(), true);
		this.preTask = task;
	}

	[DebuggerHidden]
	private IEnumerator DelayChange()
	{
		StrategyManager.<DelayChange>c__Iterator25 <DelayChange>c__Iterator = new StrategyManager.<DelayChange>c__Iterator25();
		<DelayChange>c__Iterator.<>f__this = this;
		return <DelayChange>c__Iterator;
	}

	public bool IsPlayerNearHome()
	{
		Units player = PlayerControlMgr.Instance.GetPlayer();
		return !(player == null) && player.isLive && this.IsInRange(player.transform.position, this.GetRecoveryPos((TeamType)player.teamType), 1.5f);
	}

	public bool IsInRange(Vector3 pos1, Vector3 pos2, float range)
	{
		float num = range * range;
		float sqrMagnitude = (pos1 - pos2).sqrMagnitude;
		return sqrMagnitude <= num;
	}

	private void InitAvoidence()
	{
		this._heroAvoidence.Clear();
		this._heroAvoidence.Add(0, null);
		this._heroAvoidence.Add(1, null);
	}

	public Units GetAvoidence(Units target)
	{
		return this.GetAvoidence((TeamType)target.teamType);
	}

	public Units GetAvoidence(TeamType team)
	{
		if (this._heroAvoidence.ContainsKey((int)team))
		{
			return this._heroAvoidence[(int)team];
		}
		return null;
	}

	private void HeroAvoidence()
	{
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		TeamType team = (triggerUnit.teamType != 0) ? TeamType.LM : TeamType.BL;
		new Task(this.DelayResetAvoidence((int)team, triggerUnit), true);
	}

	[DebuggerHidden]
	private IEnumerator DelayResetAvoidence(int team, Units skillCaster)
	{
		StrategyManager.<DelayResetAvoidence>c__Iterator26 <DelayResetAvoidence>c__Iterator = new StrategyManager.<DelayResetAvoidence>c__Iterator26();
		<DelayResetAvoidence>c__Iterator.skillCaster = skillCaster;
		<DelayResetAvoidence>c__Iterator.team = team;
		<DelayResetAvoidence>c__Iterator.<$>skillCaster = skillCaster;
		<DelayResetAvoidence>c__Iterator.<$>team = team;
		<DelayResetAvoidence>c__Iterator.<>f__this = this;
		return <DelayResetAvoidence>c__Iterator;
	}

	private void DefendTower()
	{
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		Units targetUnit = TriggerManager.GetTargetUnit();
		TeamType teamType = (TeamType)triggerUnit.teamType;
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits(teamType, TargetTag.Hero);
		foreach (Units current in mapUnits)
		{
			if (this.IfDefendTower(current, triggerUnit))
			{
				current.GuardTower(targetUnit, triggerUnit);
			}
		}
	}

	private bool IfDefendTower(Units defender, Units tower)
	{
		return true;
	}

	public Strategy GetStrategy(TeamType type)
	{
		if (type == TeamType.LM)
		{
			return this._lmStrategy;
		}
		return this._blStrategy;
	}

	public bool IsHomeRecovery(Units target)
	{
		return this.IsHomeRecovery((TeamType)target.teamType);
	}

	public bool IsHomeRecovery(TeamType type)
	{
		return Singleton<PvpManager>.Instance.IsInPvp || (this._homeRecovery.ContainsKey((int)type) && this._homeRecovery[(int)type]);
	}

	private void SetRecovery(TeamType type, bool recovery)
	{
		if (this._homeRecovery.ContainsKey((int)type))
		{
			this._homeRecovery[(int)type] = recovery;
		}
		else
		{
			this._homeRecovery.Add((int)type, recovery);
		}
	}

	public void UpdateStrategyRecovery(string str)
	{
		int num = 0;
		if (int.TryParse(str, out num) && num >= 301)
		{
			StrategyManager.Instance.SetRecovery(TeamType.LM, true);
			StrategyManager.Instance.SetRecovery(TeamType.BL, true);
		}
	}

	public Vector3 GetRecoveryPos(Units target)
	{
		return this.GetRecoveryPos((TeamType)target.teamType);
	}

	public Vector3 GetRecoveryPos(TeamType team)
	{
		int key;
		if (team == TeamType.LM)
		{
			SysBattleRefreshVo sysBattleRefreshVo = this.lm_brVo;
			key = this.m_lm_locationId;
		}
		else
		{
			SysBattleRefreshVo sysBattleRefreshVo = this.bl_brVo;
			key = this.m_bl_locationId;
		}
		Transform spawnPos = MapManager.Instance.GetSpawnPos(team, key);
		if (spawnPos == null)
		{
			return Vector3.zero;
		}
		return spawnPos.position;
	}

	public void UpdateBuffHero(TeamType type)
	{
		IList<Units> allLivingHero = this.GetAllLivingHero(type);
		if (allLivingHero != null)
		{
			List<Units> list = new List<Units>();
			foreach (Units current in allLivingHero)
			{
				if ((!current.isPlayer || StrategyManager.Instance.IsAuto()) && current.hp / current.hp_max <= 0.7f)
				{
					Units attackTarget = current.GetAttackTarget();
					if (attackTarget == null || (attackTarget != null && attackTarget.hp > 0.1f))
					{
						list.Add(current);
					}
				}
			}
			list.Sort((Units a, Units b) => (int)(b.hp - a.hp));
			Units value = null;
			if (list.Count > 0)
			{
				value = list[0];
			}
			if (this._buffHeros.ContainsKey(type))
			{
				this._buffHeros[type] = value;
			}
			else
			{
				this._buffHeros.Add(type, value);
			}
		}
	}

	public Units GetBuffHero(TeamType type)
	{
		if (this._buffHeros.ContainsKey(type))
		{
			return this._buffHeros[type];
		}
		return null;
	}

	public IList<Units> GetAllLivingHero(TeamType type)
	{
		return MapManager.Instance.GetMapUnits(type, TargetTag.Hero);
	}

	public void Update()
	{
	}

	public void Finish()
	{
		this.UnRegisterListerners();
		if (StrategyManager.m_instance != null)
		{
			StrategyManager.m_instance = null;
		}
		this.InitAvoidence();
		this.ResetInputState();
	}
}
