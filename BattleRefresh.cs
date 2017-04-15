using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BattleRefresh : MonoBehaviour
{
	private readonly Dictionary<int, string> _battleTriggerList = new Dictionary<int, string>();

	private readonly Dictionary<string, BattleRefreshVo> _battleRefreshList = new Dictionary<string, BattleRefreshVo>();

	private readonly List<string> _battleTimeTrigger = new List<string>();

	private readonly CoroutineManager _coroutineManager = new CoroutineManager();

	private readonly List<VTrigger> _spawnerTriggers = new List<VTrigger>();

	private readonly List<string> _clearList = new List<string>();

	private float _startTime;

	private void OnDisable()
	{
		this.StopBattleRefresh();
	}

	private void OnDestroy()
	{
		this.StopBattleRefresh();
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < this._battleTimeTrigger.Count; i++)
		{
			if (this._battleRefreshList.ContainsKey(this._battleTimeTrigger[i]) && this.UpdateTimeTrigger(this._battleRefreshList[this._battleTimeTrigger[i]]))
			{
				this._clearList.Add(this._battleTimeTrigger[i]);
			}
		}
		for (int j = 0; j < this._clearList.Count; j++)
		{
			this._battleRefreshList.Remove(this._clearList[j]);
			this._battleTimeTrigger.Remove(this._clearList[j]);
		}
	}

	private bool UpdateTimeTrigger(BattleRefreshVo br)
	{
		if (Time.time - this._startTime > br.trigerCdnFloatParam1)
		{
			br.curNumber += Time.deltaTime;
			br.curNumber2 += Time.deltaTime;
			if (br.curNumber >= br.trigerCdnFloatParam2)
			{
				br.curNumber = 0f;
				this.DoSpawnerByTime(br);
			}
			if (br.curNumber2 >= br.trigerCdnFloatParam3 && br.trigerCdnFloatParam3 != 0f)
			{
				return true;
			}
		}
		return false;
	}

	public void StartBattleRefresh()
	{
		this._startTime = Time.time;
		this._battleRefreshList.Clear();
		this._battleTriggerList.Clear();
		this._battleTimeTrigger.Clear();
		if (GlobalSettings.TestBattleRefresh)
		{
			VTrigger vTrigger = this.CreateTriger(new BattleRefreshVo("110"));
			if (vTrigger != null)
			{
				this._spawnerTriggers.Add(vTrigger);
			}
		}
		else
		{
			string curLevelId = LevelManager.CurLevelId;
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(curLevelId);
			if (dataById == null)
			{
				return;
			}
			if (dataById.refreshid != "[]")
			{
				string[] array = StringUtils.SplitVoString(dataById.refreshid, ",");
				for (int i = 0; i < array.Length; i++)
				{
					VTrigger vTrigger2 = this.CreateTriger(new BattleRefreshVo(array[i]));
					if (vTrigger2 != null)
					{
						this._spawnerTriggers.Add(vTrigger2);
					}
					StrategyManager.Instance.UpdateStrategyRecovery(array[i]);
				}
			}
		}
	}

	public void StopBattleRefresh()
	{
		this._coroutineManager.StopAllCoroutine();
		for (int i = 0; i < this._spawnerTriggers.Count; i++)
		{
			TriggerManager.DestroyTrigger(this._spawnerTriggers[i]);
		}
		this._battleRefreshList.Clear();
		this._battleTriggerList.Clear();
		this._battleTimeTrigger.Clear();
	}

	private VTrigger CreateTriger(BattleRefreshVo br)
	{
		if (!this._battleRefreshList.ContainsKey(br.br_id))
		{
			this._battleRefreshList.Add(br.br_id, br);
			VTrigger vTrigger = null;
			if (br.trigerCdn == GameEvent.TimeTigger)
			{
				this._battleTimeTrigger.Add(br.br_id);
			}
			else
			{
				vTrigger = TriggerManager.CreateGameEventTrigger(br.trigerCdn, this.GetTriggerCondition(br), new TriggerAction(this.DoSpawnerByTrigger));
				if (vTrigger != null)
				{
					this._battleTriggerList.Add(vTrigger.trigger_id, br.br_id);
				}
			}
			return vTrigger;
		}
		return null;
	}

	private void DoSpawnerByTrigger()
	{
		VTrigger trigger = TriggerManager.GetTrigger(TriggerType.GameTrigger);
		BattleRefreshVo battleRefreshVo = null;
		if (trigger != null && this._battleTriggerList.ContainsKey(trigger.trigger_id))
		{
			string key = this._battleTriggerList[trigger.trigger_id];
			if (this._battleRefreshList.ContainsKey(key))
			{
				battleRefreshVo = this._battleRefreshList[key];
			}
		}
		if (battleRefreshVo != null)
		{
			this.DoSpawner(battleRefreshVo);
			battleRefreshVo.curNumber += 1f;
			if (battleRefreshVo.curNumber >= (float)battleRefreshVo.cycleNumber && battleRefreshVo.cycleNumber != 0)
			{
				battleRefreshVo.isDestroy = true;
				this._battleRefreshList.Remove(battleRefreshVo.br_id);
			}
		}
	}

	private void DoSpawnerByTime(BattleRefreshVo br)
	{
		this.DoSpawner(br);
	}

	private void DoSpawner(BattleRefreshVo br)
	{
		int num = -1;
		Vector3 vector = Vector3.zero;
		Quaternion quaternion = Quaternion.identity;
		if (br.refreshWay == BattleRefreshWay.Position)
		{
			num = br.posIndex[0];
		}
		else if (br.refreshWay == BattleRefreshWay.RandomPos)
		{
			num = br.posIndex[UnityEngine.Random.Range(0, br.posIndex.Length - 1)];
		}
		else if (br.refreshWay == BattleRefreshWay.UnitPos)
		{
			vector = ((!(StatisticsManager.S_LastedDeadUnits != null)) ? vector : StatisticsManager.S_LastedDeadUnits.transform.position);
			quaternion = ((!(StatisticsManager.S_LastedDeadUnits != null)) ? quaternion : StatisticsManager.S_LastedDeadUnits.transform.rotation);
		}
		if (num != -1)
		{
			Transform spawnPos = MapManager.Instance.GetSpawnPos(br.teamType, num);
			vector = spawnPos.position;
			quaternion = spawnPos.rotation;
		}
		this._coroutineManager.StartCoroutine(this.DoSpawnInstance_Coroutinue(br, vector, quaternion), true);
	}

	[DebuggerHidden]
	private IEnumerator DoSpawnInstance_Coroutinue(BattleRefreshVo br, Vector3 position, Quaternion rotation)
	{
		BattleRefresh.<DoSpawnInstance_Coroutinue>c__Iterator1A3 <DoSpawnInstance_Coroutinue>c__Iterator1A = new BattleRefresh.<DoSpawnInstance_Coroutinue>c__Iterator1A3();
		<DoSpawnInstance_Coroutinue>c__Iterator1A.br = br;
		<DoSpawnInstance_Coroutinue>c__Iterator1A.position = position;
		<DoSpawnInstance_Coroutinue>c__Iterator1A.rotation = rotation;
		<DoSpawnInstance_Coroutinue>c__Iterator1A.<$>br = br;
		<DoSpawnInstance_Coroutinue>c__Iterator1A.<$>position = position;
		<DoSpawnInstance_Coroutinue>c__Iterator1A.<$>rotation = rotation;
		<DoSpawnInstance_Coroutinue>c__Iterator1A.<>f__this = this;
		return <DoSpawnInstance_Coroutinue>c__Iterator1A;
	}

	private Units DoSpawnInstance(BattleRefreshVo br, Vector3 position, Quaternion rotation, float respawnInterval = 0f, int uniqueId = 0, UnitControlType unitControlType = UnitControlType.None)
	{
		SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(br.npcID);
		Dictionary<DataType, object> dictionary = new Dictionary<DataType, object>();
		if (uniqueId != 0)
		{
			dictionary.Add(DataType.UniqueId, uniqueId);
		}
		Units units = null;
		if (br.SpawnerType == 1)
		{
			dictionary.Add(DataType.NameId, br.npcID);
			dictionary.Add(DataType.ModelId, monsterMainData.model_id);
			dictionary.Add(DataType.TeamType, (int)br.teamType);
			dictionary.Add(DataType.AttrFactor, GameManager.Instance.Spawner.GetAttrFactor(br.teamType));
			dictionary.Add(DataType.AIType, (int)br.npcAI);
			units = MapManager.Instance.SpawnMonster(dictionary, null, position, rotation);
		}
		else if (br.SpawnerType != 2)
		{
			if (br.SpawnerType != 3)
			{
				if (br.SpawnerType != 4)
				{
					if (br.SpawnerType == 5)
					{
						dictionary.Add(DataType.NameId, br.npcID);
						dictionary.Add(DataType.TeamType, (int)br.teamType);
						units = MapManager.Instance.SpawnBuffItem(dictionary, null, position, rotation, unitControlType);
						if (units != null)
						{
						}
					}
				}
			}
		}
		return units;
	}

	public Units SpawnItem(BattleRefreshVo br, int uniqueId, UnitControlType unitControlType = UnitControlType.None)
	{
		if (br == null)
		{
			return null;
		}
		int num = -1;
		Vector3 vector = Vector3.zero;
		Quaternion quaternion = Quaternion.identity;
		if (br.refreshWay == BattleRefreshWay.Position)
		{
			num = br.posIndex[0];
		}
		else if (br.refreshWay == BattleRefreshWay.RandomPos)
		{
			num = br.posIndex[UnityEngine.Random.Range(0, br.posIndex.Length - 1)];
		}
		else if (br.refreshWay == BattleRefreshWay.UnitPos)
		{
			vector = ((!(StatisticsManager.S_LastedDeadUnits != null)) ? vector : StatisticsManager.S_LastedDeadUnits.transform.position);
			quaternion = ((!(StatisticsManager.S_LastedDeadUnits != null)) ? quaternion : StatisticsManager.S_LastedDeadUnits.transform.rotation);
		}
		if (num != -1)
		{
			Transform spawnPos = MapManager.Instance.GetSpawnPos(br.teamType, num);
			vector = spawnPos.position;
			quaternion = spawnPos.rotation;
		}
		return this.DoSpawnInstance(br, vector, quaternion, 0f, uniqueId, unitControlType);
	}

	private bool DoConditionLmHeroDead()
	{
		VTrigger trigger = TriggerManager.GetTrigger(TriggerType.GameTrigger);
		if (trigger != null)
		{
			string key = this._battleTriggerList[trigger.trigger_id];
			if (this._battleRefreshList.ContainsKey(key))
			{
				BattleRefreshVo battleRefreshVo = this._battleRefreshList[key];
				if (StatisticsManager.GetDeadCount(TeamType.LM) >= battleRefreshVo.trigerCdnIntParam1)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool DoConditionBlHeroDead()
	{
		VTrigger trigger = TriggerManager.GetTrigger(TriggerType.GameTrigger);
		if (trigger != null)
		{
			string key = this._battleTriggerList[trigger.trigger_id];
			if (this._battleRefreshList.ContainsKey(key))
			{
				BattleRefreshVo battleRefreshVo = this._battleRefreshList[key];
				if (StatisticsManager.GetDeadCount(TeamType.BL) >= battleRefreshVo.trigerCdnIntParam1)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool DoConditionUnitDead()
	{
		VTrigger trigger = TriggerManager.GetTrigger(TriggerType.GameTrigger);
		if (trigger != null)
		{
			string key = this._battleTriggerList[trigger.trigger_id];
			if (this._battleRefreshList.ContainsKey(key))
			{
				BattleRefreshVo battleRefreshVo = this._battleRefreshList[key];
				if (StatisticsManager.S_LastedDeadUnits != null && StatisticsManager.S_LastedDeadUnits.npc_id == battleRefreshVo.trigerCndStrParam1)
				{
					return true;
				}
			}
		}
		return false;
	}

	private TriggerCondition GetTriggerCondition(BattleRefreshVo br)
	{
		switch (br.trigerCdn)
		{
		case GameEvent.LMHeroDead:
			return new TriggerCondition(this.DoConditionLmHeroDead);
		case GameEvent.BLHeroDead:
			return new TriggerCondition(this.DoConditionBlHeroDead);
		case GameEvent.UntiDead:
			return new TriggerCondition(this.DoConditionUnitDead);
		default:
			return null;
		}
	}
}
