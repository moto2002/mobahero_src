using BattleAttrGrowth;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AchieveManager : MonoBehaviour, IGameModule
{
	private readonly Dictionary<int, Dictionary<int, AchieveData>> _allRecords = new Dictionary<int, Dictionary<int, AchieveData>>();

	private readonly Dictionary<TeamType, int> _heroDeathDict = new Dictionary<TeamType, int>();

	private readonly Dictionary<TeamType, BattleAttrGrowth.KillData> _killDataDict = new Dictionary<TeamType, BattleAttrGrowth.KillData>();

	[SerializeField]
	private int _killMonsterNum;

	private static GamePhaseState _mGameState;

	[SerializeField]
	private bool _isVictory;

	[SerializeField]
	private bool _isFirstBlood;

	[SerializeField]
	private bool _isDoubleKill;

	[SerializeField]
	private bool _isTribleKill;

	[SerializeField]
	private bool _isLegendary;

	[SerializeField]
	private bool _isCrush;

	[SerializeField]
	private bool _isFengShen;

	[SerializeField]
	private bool _isActive;

	private EAchievementType _mAchievement;

	private VTrigger _tr1;

	private VTrigger _tr2;

	private readonly List<VTrigger> _triggerList = new List<VTrigger>();

	private readonly CoroutineManager _mCoroutineManager = new CoroutineManager();

	public static int DoubleKillCount = 2;

	public static int TripleKillCount = 3;

	public static int FourthKillCount = 4;

	public static int FifthKillCount = 5;

	public static int HexaKillCount = 6;

	public static int DashaKillCount = 3;

	public static int BaozouKillCount = 4;

	public static int WurenKillCount = 5;

	public static int ZhuzaiKillCount = 6;

	public static int GodlikeKillCount = 8;

	public static int LegendaryKillCount = 9;

	public static int TuanmieTotalCount = 2;

	public static int ZhongjieCount = 3;

	private readonly int _bossType = 7;

	private Task _checkConditionTask;

	public int AllHeroDeathNum
	{
		get;
		private set;
	}

	public bool IsActive
	{
		get
		{
			return this._isActive;
		}
	}

	public static GamePhaseState MGamePhseState
	{
		get
		{
			if (null != GameManager.Instance && GameManager.Instance.AchieveManager != null)
			{
				return AchieveManager._mGameState;
			}
			return GamePhaseState.None;
		}
	}

	public AchieveManager()
	{
		this._heroDeathDict.Clear();
		this.AllHeroDeathNum = 0;
	}

	public int GetHeroDeadCount(TeamType team)
	{
		return this._heroDeathDict[team];
	}

	public AchieveData GetAchieveData(int id, int teamType)
	{
		if (!this._allRecords.ContainsKey(teamType))
		{
			return null;
		}
		if (this._allRecords[teamType].ContainsKey(id))
		{
			return this._allRecords[teamType][id];
		}
		return null;
	}

	public Dictionary<int, AchieveData> GetAchieveDatasByTeam(int team)
	{
		if (this._allRecords.ContainsKey(team))
		{
			return this._allRecords[team];
		}
		return null;
	}

	public BattleAttrGrowth.KillData GetKillData(TeamType team)
	{
		return this._killDataDict[team];
	}

	public void InitData()
	{
		this._allRecords.Clear();
		this._killDataDict.Clear();
		for (int i = 0; i < 4; i++)
		{
			this._allRecords.Add(i, new Dictionary<int, AchieveData>());
			this._killDataDict.Add((TeamType)i, new BattleAttrGrowth.KillData((TeamType)i));
		}
		this.ReadExcel_X("1041", ref AchieveManager.DoubleKillCount);
		this.ReadExcel_X("1042", ref AchieveManager.TripleKillCount);
		this.ReadExcel_X("1043", ref AchieveManager.FourthKillCount);
		this.ReadExcel_X("1044", ref AchieveManager.FifthKillCount);
		this.ReadExcel_X("1046", ref AchieveManager.DashaKillCount);
		this.ReadExcel_X("1047", ref AchieveManager.BaozouKillCount);
		this.ReadExcel_X("1048", ref AchieveManager.WurenKillCount);
		this.ReadExcel_X("1049", ref AchieveManager.ZhuzaiKillCount);
		this.ReadExcel_X("1050", ref AchieveManager.GodlikeKillCount);
		this.ReadExcel_X("1051", ref AchieveManager.LegendaryKillCount);
		AchieveManager.TuanmieTotalCount = 2;
		AchieveManager.ZhongjieCount = 3;
	}

	private void ReadExcel(string rowId, ref int val)
	{
		SysPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPromptVo>(rowId);
		if (dataById != null)
		{
			string[] array = dataById.PromptCondition.Split(new char[]
			{
				'|'
			});
			if (array.Length > 1)
			{
				val = int.Parse(array[1]);
			}
			else
			{
				ClientLogger.Warn(rowId + " data in Prompt is error");
			}
		}
		else
		{
			ClientLogger.Warn(rowId + " data in Prompt is none");
		}
	}

	private void ReadExcel_X(string _promptId, ref int val)
	{
		SysPvpPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPvpPromptVo>(_promptId);
		if (dataById != null)
		{
			string[] array = dataById.PromptCondition.Split(new char[]
			{
				'|'
			});
			if (array.Length > 1)
			{
				val = int.Parse(array[1]);
			}
			else
			{
				ClientLogger.Warn(_promptId + " data in PvpPrompt is error");
			}
		}
		else
		{
			ClientLogger.Warn(_promptId + " data in PvpPrompt is none");
		}
	}

	private void ResetData()
	{
		this._allRecords.Clear();
		this._killMonsterNum = 0;
		this._isVictory = false;
		this._isFirstBlood = false;
		this._isDoubleKill = false;
		this._isTribleKill = false;
		this._isLegendary = false;
		this._isCrush = false;
		this._isFengShen = false;
		this.AllHeroDeathNum = 0;
		this._heroDeathDict.Clear();
	}

	public void Active()
	{
		this._isActive = true;
	}

	public void StartAchieve()
	{
		if (!this._isActive)
		{
			return;
		}
		AchieveManager._mGameState = GamePhaseState.Phase1;
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.BattlePhase1);
		Units player = PlayerControlMgr.Instance.GetPlayer();
		this._triggerList.Add(TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.KillHero), -1, "Hero"));
		this._triggerList.Add(TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.KillHero), -1, "Player"));
		this._triggerList.Add(TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.MonsterKilling), -1, "Monster"));
		this._triggerList.Add(TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.PlayerDeath), player.unique_id));
		this._triggerList.Add(TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.TowerDestroyed), -1, "Building"));
		Units home = MapManager.Instance.GetHome(TeamType.BL);
		if (home != null)
		{
			this._triggerList.Add(TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.HomeDestroyed), home.unique_id));
		}
		IList<Units> tower = MapManager.Instance.GetTower(TeamType.LM);
		if (tower != null && tower.Count > 0)
		{
			this._triggerList.Add(TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.TowerDestroyed), 0, "Building"));
		}
		this._tr1 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.HeroDeath), -1, "Hero");
		this._triggerList.Add(this._tr1);
		this._tr2 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.MonsterDeath), -1, "Monster");
		this._triggerList.Add(this._tr2);
	}

	private void EndAchieve()
	{
		this._isActive = false;
		AchieveManager._mGameState = GamePhaseState.None;
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.BattlePhase0);
		this._mCoroutineManager.StopCoroutine(this._checkConditionTask);
		for (int i = 0; i < this._triggerList.Count; i++)
		{
			TriggerManager.DestroyTrigger(this._triggerList[i]);
		}
		this._triggerList.Clear();
		this.ResetData();
		foreach (BattleAttrGrowth.KillData current in this._killDataDict.Values)
		{
			current.ResetData();
		}
	}

	private void MonsterKilling()
	{
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		Units targetUnit = TriggerManager.GetTargetUnit();
		if (triggerUnit && targetUnit && (targetUnit.CompareTag("Hero") || targetUnit.CompareTag("Player")) && triggerUnit.CompareTag("Monster"))
		{
			TeamType teamType = (TeamType)targetUnit.teamType;
			int unique_id = targetUnit.unique_id;
			string npc_id = targetUnit.npc_id;
			this.CheckRecords(teamType, unique_id, npc_id, string.Empty);
			this._allRecords[(int)teamType][unique_id].UpdateKillingMonsterData();
		}
		SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(triggerUnit.npc_id);
		if (monsterMainData != null && monsterMainData.item_type == this._bossType)
		{
			TeamType teamType2 = (TeamType)targetUnit.teamType;
			AchieveData.UpdateEpicMonster(teamType2);
		}
	}

	private void IncHeroDeath(TeamType team)
	{
		this.AllHeroDeathNum++;
		if (this._heroDeathDict.ContainsKey(team))
		{
			Dictionary<TeamType, int> heroDeathDict;
			Dictionary<TeamType, int> expr_25 = heroDeathDict = this._heroDeathDict;
			int num = heroDeathDict[team];
			expr_25[team] = num + 1;
		}
		else
		{
			this._heroDeathDict[team] = 1;
		}
	}

	public void BrocastAchievement(int attackId, int deathId, KillType killtype, List<int> helpers, int killWithTime, int killNoTime, string typeId)
	{
		Units units = null;
		if (attackId != 0)
		{
			units = MapManager.Instance.GetUnit(attackId);
		}
		Units unit = MapManager.Instance.GetUnit(deathId);
		if (unit == null)
		{
			ClientLogger.Error("Can't get units: " + deathId);
			return;
		}
		TeamType teamType = (TeamType)unit.teamType;
		int unique_id = unit.unique_id;
		string npc_id = unit.npc_id;
		bool flag = teamType != (TeamType)PlayerControlMgr.Instance.GetPlayer().teamType;
		if (attackId == 0)
		{
			SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(typeId);
			if (monsterMainData != null)
			{
				int item_type = monsterMainData.item_type;
				EntityType attackerType = EntityType.None;
				string promptId = (!flag) ? "1100" : "1099";
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					promptId = ((teamType != TeamType.BL) ? ((teamType != TeamType.LM) ? "1103" : "1102") : "1101");
				}
				if (item_type == 1)
				{
					attackerType = EntityType.Monster;
				}
				else if (item_type == 3 || item_type == this._bossType)
				{
					attackerType = EntityType.Creep;
				}
				else if (item_type == 2 || item_type == 4)
				{
					attackerType = EntityType.Tower;
				}
				UIMessageBox.ShowKillPrompt(promptId, monsterMainData.npc_id, npc_id, attackerType, EntityType.None, string.Empty, string.Empty, units.TeamType, unit.TeamType);
				return;
			}
			UnityEngine.Debug.LogError("can't get monster data with type id:" + typeId);
			return;
		}
		else
		{
			if (units == null)
			{
				ClientLogger.Error("Can't get units: " + attackId);
				return;
			}
			this.IncHeroDeath(teamType);
			TeamType teamType2 = (TeamType)units.teamType;
			int unique_id2 = units.unique_id;
			string npc_id2 = units.npc_id;
			SysPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPromptVo>("201");
			if (dataById != null)
			{
				float icon_time = dataById.icon_time;
			}
			List<string> list = new List<string>();
			if (helpers != null)
			{
				foreach (int current in helpers)
				{
					Units unit2 = MapManager.Instance.GetUnit(current);
					if (unit2 != null)
					{
						list.Add(unit2.npc_id);
					}
					else
					{
						ClientLogger.Error("can't get hero with id:" + current);
					}
				}
			}
			HUDModuleMsgTools.CallBattleMsg_SiderTipsModule_Kill(npc_id2, npc_id, flag, list, units.TeamType, unit.TeamType);
			AchieveData achieveData = new AchieveData(attackId, units.npc_id, (TeamType)units.teamType);
			AchieveData achieveData2 = new AchieveData(deathId, unit.npc_id, (TeamType)unit.teamType);
			achieveData2.unittype = units.tag;
			if (killtype == KillType.StopKill)
			{
				achieveData.CheckAchievemtCondition(achieveData, achieveData2, KillType.StopKill);
				return;
			}
			if (killtype == KillType.FirstBoold)
			{
				achieveData.ContinusKillNoTime = 1;
				achieveData.ContinusKillWithTime = 1;
			}
			else
			{
				achieveData.ContinusKillNoTime = killNoTime;
				achieveData.ContinusKillWithTime = killWithTime;
			}
			achieveData.CheckAchievemtCondition(achieveData, achieveData2, killtype);
			return;
		}
	}

	[DebuggerHidden]
	private IEnumerator DelayBrocast(AchieveData invoker, AchieveData target, KillType killtype)
	{
		AchieveManager.<DelayBrocast>c__Iterator1A5 <DelayBrocast>c__Iterator1A = new AchieveManager.<DelayBrocast>c__Iterator1A5();
		<DelayBrocast>c__Iterator1A.invoker = invoker;
		<DelayBrocast>c__Iterator1A.target = target;
		<DelayBrocast>c__Iterator1A.killtype = killtype;
		<DelayBrocast>c__Iterator1A.<$>invoker = invoker;
		<DelayBrocast>c__Iterator1A.<$>target = target;
		<DelayBrocast>c__Iterator1A.<$>killtype = killtype;
		return <DelayBrocast>c__Iterator1A;
	}

	public void KillHero()
	{
		if (LevelManager.Instance.IsPvpBattleType)
		{
			return;
		}
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		Units units = TriggerManager.GetTargetUnit();
		if (triggerUnit && units && (triggerUnit.CompareTag("Hero") || triggerUnit.CompareTag("Player")))
		{
			TeamType teamType = (TeamType)triggerUnit.teamType;
			int unique_id = triggerUnit.unique_id;
			string npc_id = triggerUnit.npc_id;
			bool flag = teamType != (TeamType)PlayerControlMgr.Instance.GetPlayer().teamType;
			string summonerName = triggerUnit.summonerName;
			this.CheckRecords(teamType, unique_id, npc_id, summonerName);
			if (!units.CompareTag("Player") && !units.CompareTag("Hero"))
			{
				Units lastHurtHero = triggerUnit.LastHurtHero;
				if (lastHurtHero == null)
				{
					EntityType attackerType = EntityType.None;
					if (units.CompareTag("Monster"))
					{
						attackerType = EntityType.Monster;
						Monster monster = units as Monster;
						if (monster != null && monster.FromNeutralMonster)
						{
							SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(units.npc_id);
							if (monsterMainData != null)
							{
								string stringById = LanguageManager.Instance.GetStringById(monsterMainData.name);
							}
							attackerType = EntityType.Creep;
						}
					}
					else if (units.CompareTag("Building") || units.CompareTag("Home"))
					{
						attackerType = EntityType.Tower;
					}
					string promptId = (!flag) ? "1100" : "1099";
					if (Singleton<PvpManager>.Instance.IsObserver)
					{
						promptId = ((teamType != TeamType.BL) ? ((teamType != TeamType.LM) ? "1103" : "1102") : "1101");
					}
					if (LevelManager.Instance.IsPvpBattleType)
					{
						UIMessageBox.ShowKillPrompt(promptId, units.npc_id, triggerUnit.npc_id, attackerType, EntityType.None, units.summonerName, triggerUnit.summonerName, units.TeamType, triggerUnit.TeamType);
					}
					else
					{
						UIMessageBox.ShowKillPrompt(promptId, units.npc_id, npc_id, attackerType, EntityType.None, string.Empty, string.Empty, TeamType.None, TeamType.None);
					}
					this._allRecords[(int)teamType][unique_id].UpdateKillingHeroData(this._allRecords[(int)teamType][unique_id]);
					return;
				}
				units = lastHurtHero;
				units.AddKillHeroNum();
				if (units.tag == "Player" && !StatisticsManager.userHeroFirstBlood && StatisticsManager.canSetHeroFirstBlood)
				{
					StatisticsManager.userHeroFirstBlood = true;
					StatisticsManager.FirstBloodUnitUniqueId = units.unique_id;
					StatisticsManager.canSetHeroFirstBlood = false;
				}
				else if (units.tag == "Hero" && !StatisticsManager.userHeroFirstBlood && StatisticsManager.canSetHeroFirstBlood)
				{
					StatisticsManager.canSetHeroFirstBlood = false;
					StatisticsManager.FirstBloodUnitUniqueId = units.unique_id;
				}
			}
			this.IncHeroDeath(teamType);
			TeamType teamType2 = (TeamType)units.teamType;
			int unique_id2 = units.unique_id;
			string npc_id2 = units.npc_id;
			string summonerName2 = units.summonerName;
			this.CheckRecords(teamType2, unique_id2, npc_id2, summonerName2);
			this._allRecords[(int)teamType2][unique_id2].UpdateKillingHeroData(this._allRecords[(int)teamType][unique_id]);
			SysPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPromptVo>("201");
			if (dataById != null)
			{
				float icon_time = dataById.icon_time;
			}
			HUDModuleMsgTools.CallBattleMsg_SiderTipsModule_Kill(npc_id2, npc_id, flag, triggerUnit.AssistantList, units.TeamType, triggerUnit.TeamType);
			foreach (Units current in triggerUnit.m_assistantDict.Keys)
			{
				if (current != null)
				{
					Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitKillAndAssist, current, null, null);
				}
			}
			if (this.AllHeroDeathNum == 1)
			{
				this._allRecords[(int)teamType2][unique_id2].CheckAchievemtCondition(null, this._allRecords[(int)teamType][unique_id], KillType.Normal);
			}
			else
			{
				this._checkConditionTask = this._mCoroutineManager.StartCoroutine(this.CheckCondition_Coroutine(teamType2, unique_id2, teamType, unique_id), true);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator CheckCondition_Coroutine(TeamType attackerTeam, int attackerId, TeamType targetTeam, int targetId)
	{
		AchieveManager.<CheckCondition_Coroutine>c__Iterator1A6 <CheckCondition_Coroutine>c__Iterator1A = new AchieveManager.<CheckCondition_Coroutine>c__Iterator1A6();
		<CheckCondition_Coroutine>c__Iterator1A.attackerTeam = attackerTeam;
		<CheckCondition_Coroutine>c__Iterator1A.attackerId = attackerId;
		<CheckCondition_Coroutine>c__Iterator1A.targetTeam = targetTeam;
		<CheckCondition_Coroutine>c__Iterator1A.targetId = targetId;
		<CheckCondition_Coroutine>c__Iterator1A.<$>attackerTeam = attackerTeam;
		<CheckCondition_Coroutine>c__Iterator1A.<$>attackerId = attackerId;
		<CheckCondition_Coroutine>c__Iterator1A.<$>targetTeam = targetTeam;
		<CheckCondition_Coroutine>c__Iterator1A.<$>targetId = targetId;
		<CheckCondition_Coroutine>c__Iterator1A.<>f__this = this;
		return <CheckCondition_Coroutine>c__Iterator1A;
	}

	private void CheckRecords(TeamType team, int id, string name, string summerName = "")
	{
		if (this._allRecords.ContainsKey((int)team))
		{
			if (!this._allRecords[(int)team].ContainsKey(id))
			{
				AchieveData achieveData = new AchieveData(id, name, team);
				achieveData.SummerName = summerName;
				this._allRecords[(int)team].Add(id, achieveData);
			}
		}
		else
		{
			ClientLogger.Error("cannot be here");
		}
	}

	public void UpdateKillHeroData(Units killer, Units dead)
	{
		if (killer && dead && killer.isHero && dead.isHero)
		{
			this.UpdateHeroData(this._killDataDict[killer.TeamType], this._killDataDict[dead.TeamType], killer);
		}
	}

	public void UpdateMonsterKillData(Units attacker)
	{
		if (attacker != null && attacker.isHero)
		{
			this._killDataDict[attacker.TeamType].AddMonsterKill(attacker.unique_id);
		}
	}

	private void UpdateHeroData(BattleAttrGrowth.KillData attackerData, BattleAttrGrowth.KillData dead, Units attacker)
	{
		attackerData.AddHeroKill(attacker.unique_id);
		if (dead.TitleLevel >= 2)
		{
			attackerData.AddFinishKillingTimes();
		}
		dead.OnHeroDeath();
	}

	private void TowerDestroyed()
	{
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		Units targetUnit = TriggerManager.GetTargetUnit();
		if (!triggerUnit || !targetUnit)
		{
			return;
		}
		TeamType teamType = (TeamType)targetUnit.teamType;
		AchieveData.UpdateTowerDestroyByTeam(teamType);
		int unique_id = targetUnit.unique_id;
		string npc_id = targetUnit.npc_id;
		string summonerName = targetUnit.summonerName;
		if (targetUnit.isHero)
		{
			this.CheckRecords(teamType, unique_id, npc_id, summonerName);
		}
		if (targetUnit.isHero && this._allRecords[(int)teamType].ContainsKey(unique_id))
		{
			this._allRecords[(int)teamType][unique_id].UpdateTowerDestroy();
		}
	}

	private void HomeDestroyed()
	{
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		if (!triggerUnit)
		{
			return;
		}
		int starnums = this.GetStarnums();
	}

	private void PlayerDeath()
	{
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		if (triggerUnit)
		{
		}
	}

	private void HeroDeath()
	{
		AchieveManager._mGameState = GamePhaseState.Phase3;
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.BattlePhase3);
		TriggerManager.DestroyTrigger(this._tr1);
		TriggerManager.DestroyTrigger(this._tr2);
	}

	private void MonsterDeath()
	{
		this._killMonsterNum++;
		if (this._killMonsterNum >= 3)
		{
			AchieveManager._mGameState = GamePhaseState.Phase2;
			Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.BattlePhase2);
			TriggerManager.DestroyTrigger(this._tr2);
		}
	}

	public int GetStarnums()
	{
		int num = 0;
		if (this._isVictory)
		{
			num++;
		}
		if (this._isFirstBlood)
		{
			num++;
		}
		if (this._isDoubleKill)
		{
			num++;
		}
		if (this._isTribleKill)
		{
			num++;
		}
		if (this._isLegendary)
		{
			num++;
		}
		if (this._isCrush)
		{
			num = 5;
		}
		if (this._isFengShen)
		{
			num = 5;
		}
		if (num > 5)
		{
			num = 5;
		}
		return num;
	}

	public void Init()
	{
		MobaMessageManager.RegistMessage((ClientMsg)25036, new MobaMessageFunc(this.OnPlayerAttached));
		if (LevelManager.Instance.IsZyBattleType || LevelManager.CurBattleType == 2 || LevelManager.CurBattleType == 9 || LevelManager.Instance.IsPvpBattleType || LevelManager.CurBattleType == 8 || LevelManager.CurBattleType == 7 || LevelManager.CurBattleType == 5 || LevelManager.CurBattleType == 4)
		{
			this.InitData();
			this.Active();
		}
	}

	public void Uninit()
	{
		AchieveData.Clear();
		this.EndAchieve();
		MobaMessageManager.UnRegistMessage((ClientMsg)25036, new MobaMessageFunc(this.OnPlayerAttached));
	}

	private void OnPlayerAttached(MobaMessage msg)
	{
		if (this.IsActive)
		{
			this.StartAchieve();
		}
	}

	public void OnGameStateChange(GameState oldState, GameState newState)
	{
	}
}
