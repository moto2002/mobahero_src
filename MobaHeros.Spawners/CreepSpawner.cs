using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace MobaHeros.Spawners
{
	public class CreepSpawner : Singleton<CreepSpawner>
	{
		private class CreepInfo
		{
			public string CreepId;
		}

		private class CreepGroup
		{
			public List<Units> Monsters = new List<Units>();

			public List<string> CreepIds = new List<string>();

			public int InitialUniqueId
			{
				get;
				set;
			}

			public void Sleep()
			{
				this.Monsters.ForEach(delegate(Units x)
				{
					Monster monster = x as Monster;
					if (monster)
					{
						monster.Sleep();
					}
				});
			}

			public void Wakeup(bool direct = false)
			{
				this.Monsters.ForEach(delegate(Units x)
				{
					Monster monster = x as Monster;
					if (monster)
					{
						monster.Wakeup(direct);
					}
				});
			}

			public void Appear()
			{
				this.Monsters.ForEach(delegate(Units x)
				{
					Monster monster = x as Monster;
					if (monster)
					{
						monster.Appear();
					}
				});
			}
		}

		private readonly Dictionary<int, CreepSpawner.CreepGroup> _creepGroups = new Dictionary<int, CreepSpawner.CreepGroup>();

		private static readonly Dictionary<int, CreepSpawner.CreepInfo> _creepInfos = new Dictionary<int, CreepSpawner.CreepInfo>();

		private List<string> _NPCIdList = new List<string>();

		private List<CreepSpawner.CreepGroup> _clearGroups = new List<CreepSpawner.CreepGroup>();

		private bool _enableLog = true;

		private CoroutineManager _coroutineManager = new CoroutineManager();

		private VTrigger _deathTrigger;

		private List<Task> _allTasks = new List<Task>();

		private bool isFirstSpawn = true;

		private float lastSleepTime;

		public void AddNPCId(string npcId)
		{
			if (this._NPCIdList != null && !this._NPCIdList.Contains(npcId))
			{
				this._NPCIdList.Add(npcId);
			}
		}

		public bool IsCreep(Units units)
		{
			return this._NPCIdList != null && this._NPCIdList.Contains(units.npc_id);
		}

		public void RemoveCreep(Units unit)
		{
			this._coroutineManager.StartCoroutine(this.RemoveCreep_Coroutine(unit), true);
		}

		[DebuggerHidden]
		private IEnumerator RemoveCreep_Coroutine(Units unit)
		{
			CreepSpawner.<RemoveCreep_Coroutine>c__Iterator1BC <RemoveCreep_Coroutine>c__Iterator1BC = new CreepSpawner.<RemoveCreep_Coroutine>c__Iterator1BC();
			<RemoveCreep_Coroutine>c__Iterator1BC.unit = unit;
			<RemoveCreep_Coroutine>c__Iterator1BC.<$>unit = unit;
			<RemoveCreep_Coroutine>c__Iterator1BC.<>f__this = this;
			return <RemoveCreep_Coroutine>c__Iterator1BC;
		}

		public string GetCreepId(Units unit)
		{
			if (CreepSpawner._creepInfos.ContainsKey(unit.unique_id))
			{
				return CreepSpawner._creepInfos[unit.unique_id].CreepId;
			}
			return null;
		}

		public void Initialize()
		{
			if (this._enableLog)
			{
			}
			this._deathTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.OnUnitDead), -1, "Monster");
			this.isFirstSpawn = true;
			this.lastSleepTime = 0f;
		}

		public void Uninitialize()
		{
			if (this._enableLog)
			{
			}
			this._creepGroups.Clear();
			CreepSpawner._creepInfos.Clear();
			this._NPCIdList.Clear();
			TriggerManager.DestroyTrigger(this._deathTrigger);
			foreach (Task current in this._allTasks)
			{
				if (current != null)
				{
					current.Stop();
				}
			}
			this._allTasks.Clear();
		}

		private void CheckCreateSummon(Units attacker, Units deadUnit)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return;
			}
			if (!CreepSpawner._creepInfos.ContainsKey(deadUnit.unique_id))
			{
				return;
			}
			SysBattleMonsterCreepVo creepConfig = CreepSpawner.GetCreepConfig(CreepSpawner._creepInfos[deadUnit.unique_id].CreepId);
			this.OnReward(creepConfig, attacker);
			if (creepConfig != null && StringUtils.CheckValid(creepConfig.summons))
			{
				this.CreateSummon(creepConfig.summons, attacker.teamType, MapManager.Instance.KeyAlloc.Get());
			}
		}

		public static SysBattleMonsterCreepVo GetReliveCreepVo(int uid)
		{
			if (!CreepSpawner._creepInfos.ContainsKey(uid))
			{
				return null;
			}
			return CreepSpawner.GetCreepConfig(CreepSpawner._creepInfos[uid].CreepId);
		}

		private void OnReward(SysBattleMonsterCreepVo vo, Units attacker)
		{
			if (vo == null)
			{
				return;
			}
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return;
			}
			if (StringUtils.CheckValid(vo.got_buff))
			{
				string[] stringValue = StringUtils.GetStringValue(vo.got_buff, '|');
				int num = int.Parse(stringValue[0]);
				if (num == 1)
				{
					IList<Units> mapUnits = MapManager.Instance.GetMapUnits((TeamType)attacker.teamType, global::TargetTag.Hero);
					if (mapUnits != null)
					{
						foreach (Units current in mapUnits)
						{
							ActionManager.AddBuff(stringValue[1], current, current, true, string.Empty);
						}
					}
				}
				else
				{
					ActionManager.AddBuff(stringValue[1], attacker, attacker, true, string.Empty);
				}
			}
		}

		private void OnUnitDead()
		{
			Units triggerUnit = TriggerManager.GetTriggerUnit();
			Units targetUnit = TriggerManager.GetTargetUnit();
			this.CheckCreateSummon(targetUnit, triggerUnit);
			this._clearGroups.Clear();
			foreach (CreepSpawner.CreepGroup current in this._creepGroups.Values)
			{
				if (current.Monsters.Contains(triggerUnit))
				{
					current.Monsters.Remove(triggerUnit);
					if (current.Monsters.Count == 0)
					{
						this._clearGroups.Add(current);
					}
				}
			}
			for (int i = 0; i < this._clearGroups.Count; i++)
			{
				this.OnGroupClear(this._clearGroups[i]);
			}
		}

		public void CreateCreeps(List<string> creepIds, int startUniqueId)
		{
			if (this._enableLog)
			{
			}
			CreepSpawner.CreepGroup creepGroup = new CreepSpawner.CreepGroup();
			creepGroup.InitialUniqueId = startUniqueId;
			creepGroup.CreepIds = creepIds;
			int num = startUniqueId;
			int i = 0;
			while (i < creepIds.Count)
			{
				try
				{
					string text = creepIds[i];
					if (this._enableLog)
					{
					}
					SysBattleMonsterCreepVo dataById = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(text);
					if (dataById == null)
					{
						ClientLogger.Error("CreateCreeps: cannot found #" + text);
					}
					else
					{
						string[] stringValue = StringUtils.GetStringValue(dataById.monsters, '|');
						int num2 = int.Parse(stringValue[1]);
						EntityVo entityVo = new EntityVo(EntityType.Monster, stringValue[0], num2, 0, string.Empty, "Default", 0)
						{
							uid = num
						};
						this.AddNPCId(stringValue[0]);
						SpawnUtility spawnUtility = GameManager.Instance.Spawner.GetSpawnUtility();
						Units units = spawnUtility.SpawnInstance(entityVo, "Monster", TeamType.Neutral, num2, "[]", null, UnitControlType.None, UnitType.None);
						if (!units)
						{
							ClientLogger.Error("CreateCreeps: cannot create " + entityVo.npc_id);
						}
						else
						{
							units.SetOrigin(true, text, 0);
							units.SetIsMonsterCreep(true);
							creepGroup.Monsters.Add(units);
						}
						if (CreepSpawner._creepInfos.ContainsKey(num))
						{
							ClientLogger.Error("CreateCreeps: unique id conflicts #" + num);
						}
						else
						{
							CreepSpawner._creepInfos.Add(num, new CreepSpawner.CreepInfo
							{
								CreepId = text
							});
						}
						if (this._enableLog)
						{
						}
					}
				}
				catch (Exception e)
				{
					ClientLogger.LogException(e);
				}
				i++;
				num++;
			}
			if (this._creepGroups.ContainsKey(startUniqueId))
			{
				ClientLogger.Error("Already existing group #" + startUniqueId);
			}
			this._creepGroups[startUniqueId] = creepGroup;
		}

		private void CreateSummon(string creepId, int teamType, int uniqueId)
		{
			if (this._enableLog)
			{
			}
			try
			{
				SysBattleMonsterCreepVo dataById = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(creepId);
				if (dataById == null)
				{
					ClientLogger.Error("CreateSummon: cannot found #" + creepId);
				}
				else
				{
					string[] stringValue = StringUtils.GetStringValue(dataById.monsters, '|');
					int num = int.Parse(stringValue[1]);
					EntityVo entityVo = new EntityVo(EntityType.Monster, stringValue[0], num, 0, string.Empty, "Default", 0)
					{
						uid = uniqueId
					};
					this.AddNPCId(stringValue[0]);
					SpawnUtility spawnUtility = GameManager.Instance.Spawner.GetSpawnUtility();
					SpawnUtility arg_A7_0 = spawnUtility;
					Transform spawnPos = MapManager.Instance.GetSpawnPos(TeamType.Neutral, num);
					Units units = arg_A7_0.SpawnInstance(entityVo, "Monster", (TeamType)teamType, num, "[]", spawnPos, UnitControlType.None, UnitType.None);
					if (!units)
					{
						ClientLogger.Error("CreateSummon: cannot create " + entityVo.npc_id);
					}
					else
					{
						if (CreepSpawner._creepInfos.ContainsKey(uniqueId))
						{
							ClientLogger.Error(string.Concat(new object[]
							{
								"CreateSummon: unique id conflicts #",
								uniqueId,
								" ",
								units.name
							}));
						}
						else
						{
							CreepSpawner._creepInfos.Add(uniqueId, new CreepSpawner.CreepInfo
							{
								CreepId = creepId
							});
						}
						if (this._enableLog)
						{
						}
					}
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		[Obsolete("debug only")]
		public void SleepAllGroups()
		{
			foreach (CreepSpawner.CreepGroup current in this._creepGroups.Values)
			{
				current.Sleep();
			}
		}

		[Obsolete("debug only")]
		public void WakeupAllGroups()
		{
			foreach (CreepSpawner.CreepGroup current in this._creepGroups.Values)
			{
				current.Wakeup(false);
			}
		}

		public void StartSpawn(string spawnInfo)
		{
			if (!StringUtils.CheckValid(spawnInfo))
			{
				return;
			}
			string[] stringValue = StringUtils.GetStringValue(spawnInfo, ',');
			string[] array = stringValue;
			for (int i = 0; i < array.Length; i++)
			{
				string str = array[i];
				string[] stringValue2 = StringUtils.GetStringValue(str, '|');
				if (stringValue2.Length > 0)
				{
					SysBattleMonsterCreepVo creepConfig = CreepSpawner.GetCreepConfig(stringValue2[0]);
					if (creepConfig == null)
					{
						ClientLogger.Error("StartSpawn: cannot found #" + stringValue2[0]);
					}
					else
					{
						this.DoSpawn(stringValue2.ToList<string>(), (float)creepConfig.delay_time);
					}
				}
			}
		}

		private void DoSpawn(List<string> idList, float delay)
		{
			Task item = this._coroutineManager.StartCoroutine(this.Spawn_Coroutine(idList, delay), true);
			this._allTasks.Add(item);
		}

		[DebuggerHidden]
		private IEnumerator Spawn_Coroutine(List<string> idList, float delay)
		{
			CreepSpawner.<Spawn_Coroutine>c__Iterator1BD <Spawn_Coroutine>c__Iterator1BD = new CreepSpawner.<Spawn_Coroutine>c__Iterator1BD();
			<Spawn_Coroutine>c__Iterator1BD.delay = delay;
			<Spawn_Coroutine>c__Iterator1BD.idList = idList;
			<Spawn_Coroutine>c__Iterator1BD.<$>delay = delay;
			<Spawn_Coroutine>c__Iterator1BD.<$>idList = idList;
			<Spawn_Coroutine>c__Iterator1BD.<>f__this = this;
			return <Spawn_Coroutine>c__Iterator1BD;
		}

		private static SysBattleMonsterCreepVo GetCreepConfig(string id)
		{
			return BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(id);
		}

		private void OnGroupClear(CreepSpawner.CreepGroup creep)
		{
			this._creepGroups.Remove(creep.InitialUniqueId);
			this.FreshGroup(creep);
		}

		private void FreshGroup(CreepSpawner.CreepGroup creep)
		{
			SysBattleMonsterCreepVo creepConfig = CreepSpawner.GetCreepConfig(creep.CreepIds[0]);
			if (creepConfig == null)
			{
				ClientLogger.Error("StartSpawn: cannot found #" + creep.CreepIds[0]);
				return;
			}
			this.DoSpawn(creep.CreepIds, (float)creepConfig.refresh_time);
		}
	}
}
