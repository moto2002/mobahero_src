using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using Common;
using MobaFrame.SkillAction;
using MobaHeros.AI;
using MobaHeros.Spawners;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobaHeros.Pvp
{
	public static class PvpProtocolTools
	{
		private const string LogTag = "pvp";

		public static TeamType GroupToTeam(int group)
		{
			if (group == 0)
			{
				return TeamType.LM;
			}
			if (group == 1)
			{
				return TeamType.BL;
			}
			if (group == 2)
			{
				return TeamType.Neutral;
			}
			if (group == 3)
			{
				return TeamType.Team_3;
			}
			return TeamType.None;
		}

		public static TeamType GetTeam(this ReadyPlayerSampleInfo info)
		{
			return PvpProtocolTools.GroupToTeam((int)info.group);
		}

		public static Units CreateMapItem(MapItemInfo info, UnitControlType unitControlType = UnitControlType.None)
		{
			if (info == null)
			{
				return null;
			}
			SkillUnitData vo = Singleton<SkillUnitDataMgr>.Instance.GetVo(info.uniTypeId);
			if (vo == null)
			{
				return null;
			}
			Dictionary<DataType, object> dictionary = new Dictionary<DataType, object>();
			dictionary.Add(DataType.NameId, info.uniTypeId);
			dictionary.Add(DataType.TeamType, (TeamType)info.group);
			if (info.unitId != 0)
			{
				dictionary.Add(DataType.UniqueId, info.unitId);
			}
			Units units;
			if (vo.IsBloodBall)
			{
				units = MapManager.Instance.SpawnBuffItem(dictionary, null, MoveController.SVectgor3ToVector3(info.burnPos), Quaternion.identity, unitControlType);
			}
			else
			{
				Units unit = MapManager.Instance.GetUnit(info.callUnitId);
				units = MapManager.Instance.SpawnSkillUnit(dictionary, null, MoveController.SVectgor3ToVector3(info.burnPos), Quaternion.identity, unitControlType, unit, info.callSkillId);
				HighEffectData vo2 = Singleton<HighEffectDataManager>.Instance.GetVo(info.hieffId);
				if (vo2 != null)
				{
					List<Units> list = new List<Units>();
					if (info.targetUnitIds != null)
					{
						for (int i = 0; i < info.targetUnitIds.Count; i++)
						{
							Units unit2 = MapManager.Instance.GetUnit(info.targetUnitIds[i]);
							list.Add(unit2);
						}
					}
					BornUnitAction.SetPosition(units, list, unit, vo2, MoveController.SVectgor3ToVector3(info.burnPos));
				}
			}
			if (units)
			{
				units.UpdateVisible();
			}
			return units;
		}

		public static Units CreateMapItem(UnitRuntimeInfo info, UnitControlType unitControlType = UnitControlType.None)
		{
			if (info == null)
			{
				return null;
			}
			if (info.baseUnitInfo == null)
			{
				return null;
			}
			if (info.baseUnitInfo.skillunitInfo == null)
			{
				return null;
			}
			SkillUnitData vo = Singleton<SkillUnitDataMgr>.Instance.GetVo(info.baseUnitInfo.typeId);
			if (vo == null)
			{
				return null;
			}
			Dictionary<DataType, object> dictionary = new Dictionary<DataType, object>();
			dictionary.Add(DataType.NameId, info.baseUnitInfo.typeId);
			dictionary.Add(DataType.TeamType, (TeamType)info.baseUnitInfo.group);
			if (info.baseUnitInfo.unitId != 0)
			{
				dictionary.Add(DataType.UniqueId, info.baseUnitInfo.unitId);
			}
			Units result;
			if (vo.IsBloodBall)
			{
				result = MapManager.Instance.SpawnBuffItem(dictionary, null, MoveController.SVectgor3ToVector3(info.baseUnitInfo.skillunitInfo.burnPos), Quaternion.identity, unitControlType);
			}
			else
			{
				result = MapManager.Instance.SpawnSkillUnit(dictionary, null, MoveController.SVectgor3ToVector3(info.baseUnitInfo.skillunitInfo.burnPos), Quaternion.identity, unitControlType, null, string.Empty);
			}
			return result;
		}

		public static Units CreateMonsterByUnitInfo(UnitInfo info)
		{
			Units result;
			try
			{
				if (info == null)
				{
					ClientLogger.Error("CreateMonsterByUnitInfo: info is null");
					result = null;
				}
				else if (MapManager.Instance == null)
				{
					ClientLogger.Error("MapManager.Instance is null");
					result = null;
				}
				else if (GlobalSettings.TestCreep)
				{
					Singleton<CreepSpawner>.Instance.CreateCreeps(new List<string>
					{
						"101"
					}, info.unitId);
					result = null;
				}
				else if (GlobalSettings.NoMonster)
				{
					ClientLogger.Warn("P2C_CreateUnits create monster ignored");
					result = null;
				}
				else if (info.unitType == UnitType.EyeItem)
				{
					result = PvpProtocolTools.CreateEyeItemByUnitInfo(info);
				}
				else
				{
					TeamType teamType = PvpProtocolTools.GroupToTeam((int)info.group);
					int num = -1;
					Transform transform = null;
					if (StringUtils.CheckValid(info.burnValue))
					{
						string[] stringValue = StringUtils.GetStringValue(info.burnValue, '|');
						UnitType unitType = info.unitType;
						if (unitType != UnitType.Monster)
						{
							if (unitType != UnitType.Soldier)
							{
								ClientLogger.Error("cannot be here");
							}
							else
							{
								num = int.Parse(stringValue[2]);
							}
						}
						else
						{
							num = int.Parse(stringValue[1]);
							transform = MapManager.Instance.GetSpawnPos(TeamType.Neutral, num);
						}
						if (num < 0)
						{
							ClientLogger.Error("burnValue is invalid, use position #" + info.typeId + "  " + info.burnValue);
						}
					}
					else if (info.unitType == UnitType.EyeUnit)
					{
						transform = MapManager.Instance.GetSpawnPos(TeamType.Neutral, 1);
						if (transform != null)
						{
							transform.position = new Vector3(info.position.x, info.position.y, info.position.z);
						}
					}
					else if (info.unitType == UnitType.SummonMonster || info.unitType == UnitType.BoxUnit)
					{
						transform = MapManager.Instance.GetSpawnPos((TeamType)info.group, 1);
						if (transform != null)
						{
							transform.position = new Vector3(info.position.x, info.position.y, info.position.z);
						}
					}
					else if (info.unitType == UnitType.Pet)
					{
						transform = MapManager.Instance.GetSpawnPos((TeamType)info.group, 1);
						if (transform != null)
						{
							transform.position = new Vector3(info.position.x, info.position.y, info.position.z);
						}
					}
					else if (info.unitType == UnitType.LabisiUnit)
					{
						transform = MapManager.Instance.GetSpawnPos((TeamType)info.group, 1);
						if (transform != null)
						{
							transform.position = new Vector3(info.position.x, info.position.y, info.position.z);
						}
					}
					else
					{
						ClientLogger.Error(string.Concat(new object[]
						{
							"burnValue is invalid, use default position #",
							info.typeId,
							"  utype:",
							info.unitType
						}));
					}
					Units unit = MapManager.Instance.GetUnit(info.mainHeroId);
					int skin = 0;
					if (unit != null && info.unitType == UnitType.SummonMonster)
					{
						skin = HeroSkins.GetRealHeroSkin((TeamType)unit.teamType, unit.model_id);
					}
					EntityVo npcinfo = new EntityVo(EntityType.Monster, info.typeId, num, 0, string.Empty, "Default", 0)
					{
						uid = info.unitId,
						skin = skin
					};
					if (null == GameManager.Instance)
					{
						Debug.LogError("null == GameManager.Instance");
						result = null;
					}
					else if (GameManager.Instance.Spawner == null)
					{
						Debug.LogError("null == GameManager.Instance.Spawner");
						result = null;
					}
					else
					{
						SpawnUtility spawnUtility = GameManager.Instance.Spawner.GetSpawnUtility();
						Units units = spawnUtility.SpawnInstance(npcinfo, "Monster", teamType, num, "[]", transform, UnitControlType.None, info.unitType);
						if (units == null)
						{
							ClientLogger.Error(string.Concat(new object[]
							{
								"P2C_CreateUnits create monster failed, creepId=",
								info.creepId,
								" typeid= ",
								info.typeId,
								" burnValue=",
								info.burnValue ?? "null"
							}));
							result = null;
						}
						else
						{
							if (units.UnitType == UnitType.EyeUnit)
							{
							}
							if (unit != null)
							{
								units.ParentUnit = unit;
							}
							if (units.UnitType == UnitType.EyeUnit || units.UnitType == UnitType.SummonMonster)
							{
								units.m_fLiveTime = info.liveTime;
								units.m_fLeftTime = info.liveTime;
							}
							units.SetOrigin(true, info.creepId.ToString(), info.monsterTeamId);
							units.TryAddBirthEffect();
							units.SetIsMonsterCreep(info.unitType == UnitType.Monster || info.unitType == UnitType.CreepBoss);
							PvpProtocolTools.SyncUnitLifeStateAndSkill(units, info, 0L);
							if (units.isMonster && units.skillManager != null)
							{
								units.skillManager.EnableSkills(true);
							}
							if (units != null && transform != null)
							{
								units.SetPosition(transform.position, true);
							}
							result = units;
						}
					}
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
				result = null;
			}
			return result;
		}

		public static Units CreateHeroByUnitInfo(UnitInfo info)
		{
			Units result;
			try
			{
				if (info == null)
				{
					result = null;
				}
				else
				{
					EntityVo entityVo = new EntityVo(EntityType.Hero, info.typeId, 0, 0, string.Empty, "Default", 0)
					{
						uid = info.unitId,
						level = info.level,
						hp = 99999f,
						mp = 99999f,
						effectId = string.Empty
					};
					Units unit = MapManager.Instance.GetUnit(info.mainHeroId);
					if (unit != null)
					{
						if (unit.npc_id == "Jiansheng")
						{
							entityVo.effectId = "1|Perform_jsbirth_fenshen";
						}
						else if (unit.npc_id == "Houzi")
						{
						}
					}
					SpawnUtility spawnUtility = GameManager.Instance.Spawner.GetSpawnUtility();
					Vector3 vector = MoveController.SVectgor3ToVector3(info.position);
					string userName = (!(unit == null)) ? unit.summonerName : "1";
					string summerId = "1";
					Units units = spawnUtility.SpawnPvpHero(entityVo, "Hero", (TeamType)info.group, 0, userName, summerId, new Vector3?(vector), UnitType.FenShenHero);
					if (units != null && unit != null)
					{
						units.level = unit.level;
						units.data.SetMaxHp(unit.hp_max);
						units.data.SetMaxMp(unit.mp_max);
						units.data.SetHp(unit.hp);
						units.data.SetMp(unit.mp);
						units.SetParentUnit(unit);
						units.SetMirrorState(true);
						if (unit.npc_id == "Jiansheng")
						{
							units.SetCanAIControl(true);
							units.SetCanSkill(false);
							units.effect_id = "2|Perform_jsdead_fenshen";
						}
						else if (unit.npc_id == "Houzi")
						{
							units.effect_id = "2|FenShenDeath,2|DashengS2";
							units.SetCanAIControl(false);
							units.SetCanAction(false);
							units.trans.rotation = unit.trans.rotation;
						}
						Units selectedTarget = PlayerControlMgr.Instance.GetSelectedTarget();
						if (selectedTarget != null && unit.unique_id == selectedTarget.unique_id)
						{
							PlayerControlMgr.Instance.GetPlayer().SetSelectTarget(null);
							PlayerControlMgr.Instance.GetPlayer().SetAttackTarget(null);
							PlayerControlMgr.Instance.SetSelectedTarget(null);
						}
						units.SetPosition(vector, true);
					}
					result = units;
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
				result = null;
			}
			return result;
		}

		public static Units CreateEyeItemByUnitInfo(UnitInfo info)
		{
			if (!StringUtils.CheckValid(info.burnValue))
			{
				ClientLogger.Error(string.Concat(new object[]
				{
					"burnValue is invalid, use default position #",
					info.typeId,
					"  utype:",
					info.unitType
				}));
				return null;
			}
			string[] stringValue = StringUtils.GetStringValue(info.burnValue, '|');
			if (stringValue.Length != 6)
			{
				ClientLogger.Error("eye item burnValue is stuctured wrong");
				return null;
			}
			int num = -1;
			if (!int.TryParse(stringValue[2], out num))
			{
				ClientLogger.Error("parse eye item pos failed");
				return null;
			}
			if (num < 0)
			{
				ClientLogger.Error("eye item pos invalid");
				return null;
			}
			Transform spawnPos = MapManager.Instance.GetSpawnPos(TeamType.Neutral, num);
			TeamType teamType = PvpProtocolTools.GroupToTeam((int)info.group);
			EntityVo npcinfo = new EntityVo(EntityType.Monster, info.typeId, num, 9, string.Empty, "Default", 0)
			{
				uid = info.unitId
			};
			SpawnUtility spawnUtility = GameManager.Instance.Spawner.GetSpawnUtility();
			Units units = spawnUtility.SpawnEyeItemInstance(npcinfo, "Monster", teamType, "[]", spawnPos, info.position, stringValue[4], UnitControlType.None);
			if (units == null)
			{
				ClientLogger.Error("P2C_CreateUnits create monster failed, " + StringUtils.DumpObject(info));
				return null;
			}
			units.SetOrigin(true, info.creepId.ToString(), info.monsterTeamId);
			units.TryAddBirthEffect();
			units.SetIsMonsterCreep(false);
			PvpProtocolTools.SyncUnitLifeStateAndSkill(units, info, 0L);
			if (units.isMonster && units.skillManager != null)
			{
				units.skillManager.EnableSkills(true);
			}
			return units;
		}

		public static void UpdateHeroSkins(IEnumerable<ReadyPlayerSampleInfo> allPlayers)
		{
			HeroSkins.Clear();
			foreach (ReadyPlayerSampleInfo current in allPlayers)
			{
				if (!string.IsNullOrEmpty(current.heroSkinId))
				{
					int skinIdx = 0;
					if (int.TryParse(current.heroSkinId, out skinIdx))
					{
						HeroSkins.SetHeroSkin(PvpProtocolTools.GroupToTeam((int)current.group), current.heroInfo.heroId, skinIdx);
					}
				}
			}
		}

		public static void ToDie(Units deadUnit, Units attacker, long spawnInterval)
		{
			if (deadUnit == null)
			{
				return;
			}
			float num = (float)TimeSpan.FromTicks(spawnInterval).TotalSeconds;
			PvpManager.On_KillHero(deadUnit.unique_id, 0f, num);
			if (deadUnit.IsMaster && deadUnit.isPlayer)
			{
				BattleAttrManager.Instance.SetPlayerDeathTimer(deadUnit.unique_id, num);
				Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.HeroDeathTimer, deadUnit, null, null);
				MobaMessageManager.ExecuteMsg(ClientC2C.UnitDeathTime, new ParamUnitDeathTime
				{
					reliveTime = num,
					uniqueId = deadUnit.unique_id
				}, 0f);
			}
			else if (deadUnit.isHero)
			{
				BattleAttrManager.Instance.SetPlayerDeathTimer(deadUnit.unique_id, num);
				MobaMessageManager.ExecuteMsg(ClientC2C.UnitDeathTime, new ParamUnitDeathTime
				{
					reliveTime = num,
					uniqueId = deadUnit.unique_id
				}, 0f);
			}
			if (deadUnit.isLive)
			{
				deadUnit.isLive = false;
				deadUnit.PreDeath(attacker);
				deadUnit.RealDeath(attacker);
			}
		}

		public static void DoMonsterPseudoDeath(Units inMonsterUnit, Units inAttackerUnit, int inOldGroupType, int inNewGroupType, float inHpVal, float inMpVal, string inNpcId, string inBattleMonsterCreepId)
		{
			if (inMonsterUnit == null)
			{
				return;
			}
			inMonsterUnit.PseudoDeath(inOldGroupType, inNewGroupType, inHpVal, inMpVal, inNpcId, inBattleMonsterCreepId, inAttackerUnit);
			CreepHelper.TryShowCreepSpecialEffect(inBattleMonsterCreepId, inMonsterUnit);
			CreepHelper.TryShowCreepUIMessageInfo(inAttackerUnit, inMonsterUnit, inBattleMonsterCreepId, inOldGroupType);
			Singleton<MiniMapView>.Instance.ForceUpdateMapItemByUnits(inMonsterUnit, true);
		}

		public static void HandleMsgNotifySpawnSuperSoldier(TeamType inTeam)
		{
			UIMessageBox.ShowSpawnSuperSoldierMsg(inTeam, TeamManager.MyTeam);
		}

		public static void HandleMsgNotifyMonsterCreepAiStatus(int inUnitId, EMonsterCreepAiStatus inMonsterCreepAiStatus)
		{
			Units unit = MapManager.Instance.GetUnit(inUnitId);
			if (unit != null)
			{
				unit.SetMonsterCreepAiStatus(inMonsterCreepAiStatus);
			}
		}

		public static void SyncSceneState(Dictionary<string, string> states)
		{
			if (states == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> current in states)
			{
				if (current.Key.StartsWith(SceneValueType.SceneDoor.ToString()))
				{
				}
				if (current.Key.StartsWith(SceneValueType.MonsterTeamFreshTime.ToString()))
				{
					int groupId = int.Parse(current.Key.Split(new char[]
					{
						':'
					})[1]);
					Singleton<MiniMapView>.Instance.AddMiniMapBossFreshTune(groupId, current.Value);
				}
			}
		}

		public static void SyncFightInfo(InBattleRuntimeInfo fightInfo)
		{
			if (fightInfo == null || fightInfo.teamInfos == null)
			{
				return;
			}
			if (fightInfo.frameTime > 0)
			{
				FrameSyncManager.Instance.OneFrameTime = (double)((float)fightInfo.frameTime / 1000f);
			}
			PvpProtocolTools.ResetHeroStatistic(fightInfo);
			GameManager.Instance.SurrenderMgr.SyncInfos(fightInfo.surrenderInfos);
			Singleton<PvpManager>.Instance.RoomInfo.CtrlUniqueIds.Clear();
			if (fightInfo.unitInfos == null)
			{
				return;
			}
			PvpProtocolTools.SyncSceneState(fightInfo.sceneValues);
			Singleton<PvpManager>.Instance.GameStartTime = new DateTime?(DateTime.Now - TimeSpan.FromTicks(fightInfo.gameTime));
			Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
			List<Units> list = new List<Units>();
			foreach (Units unit in allMapUnits.Values)
			{
				if ((!unit.isHero && !unit.isHome && !unit.isBuilding) || unit.MirrorState)
				{
					if (unit.isLive)
					{
						UnitRuntimeInfo unitRuntimeInfo = fightInfo.unitInfos.FirstOrDefault((UnitRuntimeInfo x) => x.baseUnitInfo.unitId == unit.unique_id);
						if (unitRuntimeInfo == null)
						{
							list.Add(unit);
						}
					}
				}
			}
			foreach (Units current in list)
			{
				MapManager.Instance.DespawnUnit(current);
			}
			for (int i = 0; i < fightInfo.unitInfos.Length; i++)
			{
				PvpProtocolTools.SyncSingleUnit(fightInfo.unitInfos[i]);
			}
			Singleton<MiniMapView>.Instance.UpdateAfterReConect();
		}

		public static void SyncSingleUnit(UnitRuntimeInfo unitInfo)
		{
			byte lifeState = unitInfo.baseUnitInfo.lifeState;
			if (lifeState == 2 || lifeState == 3)
			{
				int unitId = unitInfo.baseUnitInfo.unitId;
				PvpManager.On_KillHero(unitId, 0f, (float)TimeSpan.FromTicks(unitInfo.reliveLeftTime).TotalSeconds);
			}
			PvpProtocolTools.TryRecoverUnit(unitInfo);
		}

		private static void SetUnitState(Units unit, UnitRuntimeInfo info)
		{
			if (!unit)
			{
				return;
			}
			PvpProtocolTools.SyncUnitPosition(unit, info);
			PvpProtocolTools.SyncUnitProperties(unit, info);
			PvpProtocolTools.SyncUnitBuff(unit, info);
			PvpProtocolTools.SyncUnitLifeStateAndSkill(unit, info.baseUnitInfo, info.reliveLeftTime);
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player != null && player.teamType < 4)
			{
				unit.m_nServerVisibleState = (int)info.nVisebleState[player.teamType];
			}
			PvpProtocolTools.TrySyncMonsterCreepGroupType(unit, info);
			PvpProtocolTools.TrySyncMonsterCreepRotation(unit);
		}

		private static void TrySyncMonsterCreepGroupType(Units unit, UnitRuntimeInfo info)
		{
			if (unit == null || info == null || info.baseUnitInfo == null)
			{
				return;
			}
			if (!TagManager.CheckTag(unit, global::TargetTag.Creeps))
			{
				return;
			}
			if (unit.teamType == (int)info.baseUnitInfo.group)
			{
				return;
			}
			unit.teamType = (int)info.baseUnitInfo.group;
			unit.SetBloodBarStyle();
		}

		private static void TrySyncMonsterCreepRotation(Units inUnit)
		{
			if (inUnit == null)
			{
				return;
			}
			if (!TagManager.CheckTag(inUnit, global::TargetTag.Creeps))
			{
				return;
			}
			Quaternion spwan_rotation = inUnit.spwan_rotation;
			if (inUnit.mTransform != null)
			{
				inUnit.mTransform.rotation = Quaternion.Euler(spwan_rotation.eulerAngles);
			}
		}

		private static void SyncUnitBuff(Units unit, UnitRuntimeInfo info)
		{
			if (!unit)
			{
				return;
			}
			if (info == null)
			{
				return;
			}
			if (unit.buffManager != null)
			{
				unit.buffManager.ClearAllBuffs();
			}
			if (info.buffInfo == null)
			{
				return;
			}
			Dictionary<string, BuffRuntimeData>.Enumerator enumerator = info.buffInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int num = 0;
				while (true)
				{
					byte arg_83_0 = (byte)num;
					KeyValuePair<string, BuffRuntimeData> current = enumerator.Current;
					if (arg_83_0 >= current.Value.layer)
					{
						break;
					}
					KeyValuePair<string, BuffRuntimeData> current2 = enumerator.Current;
					ActionManager.AddBuff(current2.Key, unit, null, false, string.Empty);
					num++;
				}
				BuffManager arg_B4_0 = unit.buffManager;
				KeyValuePair<string, BuffRuntimeData> current3 = enumerator.Current;
				string arg_B4_1 = current3.Key;
				KeyValuePair<string, BuffRuntimeData> current4 = enumerator.Current;
				arg_B4_0.SetBuffCDTime(arg_B4_1, (float)current4.Value.curTime);
			}
		}

		private static void SyncUnitProperties(Units unit, UnitRuntimeInfo info)
		{
			unit.data.SetMaxHp(info.maxhp);
			unit.SetHp(info.hp);
			unit.data.SetMaxMp(info.maxmp);
			unit.SetMp(info.mp);
		}

		private static void SyncUnitPosition(Units unit, UnitRuntimeInfo info)
		{
			if (info.position != null && !unit.isBuilding)
			{
				Vector3 vector = new Vector3(info.position.x, unit.transform.position.y, info.position.z);
				if (unit.moveController != null)
				{
					unit.moveController.CurPosition = vector;
				}
				unit.SetPosition(vector, false);
				unit.transform.rotation = Quaternion.AngleAxis(info.rotateY, Vector3.up);
				unit.MoveToPoint(new Vector3?(info.targetPosition.ToVector3()), false);
			}
			else
			{
				unit.transform.position = unit.spwan_pos;
			}
		}

		private static void SyncUnitLifeStateAndSkill(Units unit, UnitInfo unitInfo, long reliveLeftTime)
		{
			if (!unit)
			{
				return;
			}
			PvpProtocolTools.SyncHeroSkills(unit, unitInfo);
			PvpProtocolTools.SyncUnitLifeState(unit, unitInfo, reliveLeftTime);
			PvpProtocolTools.SyncHeroItems(unit, unitInfo);
			PvpProtocolTools.SyncMainHeroData(unit, unitInfo);
		}

		private static void SyncUnitLifeState(Units unit, UnitInfo unitInfo, long reliveLeftTime)
		{
			try
			{
				PvpLifeState lifeState = (PvpLifeState)unitInfo.lifeState;
				if (lifeState == PvpLifeState.Dead || lifeState == PvpLifeState.WaitRelive)
				{
					PvpProtocolTools.ToDie(unit, null, reliveLeftTime);
				}
				else
				{
					if (!unit.isLive)
					{
						if (unit.isHero)
						{
							SpawnUtility spawnUtility = GameManager.Instance.Spawner.GetSpawnUtility();
							spawnUtility.RespawnPvpHero(unit);
						}
						else
						{
							ClientLogger.Error(string.Concat(new object[]
							{
								"SetUnitState: cannot relive non-hero unit #",
								unit.unique_id,
								" ",
								unit.name,
								" to state ",
								lifeState
							}));
						}
					}
					if (unit.IsMonsterCreep())
					{
						Monster monster = unit as Monster;
						if (lifeState == PvpLifeState.Unactive)
						{
							monster.Sleep();
						}
						else if (lifeState == PvpLifeState.Live)
						{
							if (monster.Sleeping.IsInState)
							{
								monster.Wakeup(false);
							}
							else
							{
								monster.Appear();
							}
						}
						else
						{
							ClientLogger.Warn(string.Concat(new object[]
							{
								"don't know how to treat ",
								unit.name,
								" with life state ",
								lifeState
							}));
						}
					}
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		private static void SyncHeroItems(Units unit, UnitInfo unitInfo)
		{
			if (!unit.isHero)
			{
				return;
			}
			if (unitInfo.heroInfo == null)
			{
				ClientLogger.Error("heroInfo is null");
				return;
			}
			List<ItemDynData> list = new List<ItemDynData>();
			if (unitInfo.heroInfo.itemInfo != null)
			{
				list.AddRange(unitInfo.heroInfo.itemInfo);
			}
			((Hero)unit).EquipPackage.SetEquipList(list);
		}

		private static void SyncMainHeroData(Units unit, UnitInfo unitInfo)
		{
			if (null == unit || unitInfo == null || !unit.isPlayer || unitInfo.heroInfo == null)
			{
				return;
			}
			if (ModelManager.Instance.Get_BattleShop_initRItems())
			{
				return;
			}
			MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_syncRItems, unitInfo.heroInfo.playerVar, false);
		}

		private static void SyncHeroSkills(Units unit, UnitInfo unitInfo)
		{
			if (!unit.isHero)
			{
				return;
			}
			if (unitInfo.heroInfo == null)
			{
				ClientLogger.Error("heroInfo is null");
				return;
			}
			try
			{
				PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(unitInfo.unitId);
				heroData.CurGold = unitInfo.heroInfo.money;
				heroData.TotalGold = unitInfo.heroInfo.totalMoney;
				heroData.CurLv = unitInfo.level;
				heroData.CurExp = unitInfo.heroInfo.exp;
				if (unit.mCmdRunningController != null)
				{
					unit.mCmdRunningController.Reset();
				}
				unit.level = unitInfo.level;
				unit.skillManager.SkillPointsLeft = unitInfo.heroInfo.skillPoint;
				if (unitInfo.heroInfo.skillInfo != null)
				{
					Dictionary<int, string> skills_index = unit.skillManager.skills_index;
					int num = 0;
					foreach (string current in unitInfo.heroInfo.skillInfo.Keys)
					{
						int num2 = -1;
						foreach (int current2 in skills_index.Keys)
						{
							if (skills_index[current2] == current)
							{
								num2 = current2;
								break;
							}
						}
						if (num2 >= 0)
						{
							string text = skills_index[num2];
							Skill skillById = unit.skillManager.getSkillById(text);
							if (skillById != null)
							{
								skillById.BulletIndex = unitInfo.heroInfo.skillInfo[current].index;
							}
							int level = (int)unitInfo.heroInfo.skillInfo[current].level;
							num += level;
							int skillIdx = unitInfo.heroInfo.skillInfo[current].skillIdx;
							unit.skillManager.UpgradeSkillLevel(text, level);
							unit.skillManager.ResetSkillSubIndex(text, skillIdx);
							float num3 = (float)unitInfo.heroInfo.skillInfo[current].cd / 1000f;
							float cd_time = unitInfo.heroInfo.skillInfo[current].chargeCD / 1000f;
							unit.SetCDTime(text, num3);
							unit.SetChargeTime(text, cd_time);
							unit.OnSkillSynced(text, unitInfo.heroInfo.skillInfo[current].useState);
							if (unit.isPlayer)
							{
								Singleton<SkillView>.Instance.SetSkillUILevel(text, level);
								Singleton<SkillView>.Instance.UpdateSkillItemCDTime(num3, text);
								Singleton<SkillView>.Instance.UpdateSkillView(text, false);
							}
						}
					}
					if (num == 0 && unit.level == 1 && unit.skillManager.SkillPointsLeft == 0 && unit.isPlayer)
					{
						unit.skillManager.SkillPointsLeft = 1;
					}
				}
				if (unit.isPlayer)
				{
					Singleton<SkillView>.Instance.SetSkillPointLeft(unitInfo.heroInfo.skillPoint);
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		private static void TryRecoverUnit(UnitRuntimeInfo info)
		{
			UnitInfo baseUnitInfo = info.baseUnitInfo;
			Units units = MapManager.Instance.GetUnit(baseUnitInfo.unitId);
			try
			{
				UnitType unitType = baseUnitInfo.unitType;
				if (unitType != UnitType.Hero)
				{
					if (unitType != UnitType.Monster)
					{
						if (unitType != UnitType.Tower && unitType != UnitType.Home)
						{
							if (unitType == UnitType.MapItem)
							{
								if (!units)
								{
									PvpProtocolTools.CreateMapItem(info, UnitControlType.None);
								}
								goto IL_179;
							}
							if (unitType != UnitType.Soldier)
							{
								if (unitType == UnitType.FenShenHero)
								{
									if (!units)
									{
										units = PvpProtocolTools.CreateHeroByUnitInfo(info.baseUnitInfo);
									}
									PvpProtocolTools.SetUnitState(units, info);
									goto IL_179;
								}
								if (unitType != UnitType.EyeUnit && unitType != UnitType.SummonMonster && unitType != UnitType.Pet && unitType != UnitType.LabisiUnit)
								{
									goto IL_179;
								}
							}
						}
						else
						{
							units = MapManager.Instance.GetUnit(baseUnitInfo.unitId);
							if (!units)
							{
								ClientLogger.Error("TryCreateUnit: tower not found - " + StringUtils.DumpObject(baseUnitInfo));
								return;
							}
							PvpProtocolTools.SetUnitState(units, info);
							goto IL_179;
						}
					}
					if (!units)
					{
						units = PvpProtocolTools.CreateMonsterByUnitInfo(info.baseUnitInfo);
					}
					PvpProtocolTools.SetUnitState(units, info);
				}
				else
				{
					if (!units)
					{
						units = MapManager.Instance.TryFetchRecycledUnit(baseUnitInfo.unitId);
						if (!units)
						{
							ClientLogger.Error("TryCreateUnit: hero not found - " + StringUtils.DumpObject(baseUnitInfo));
							return;
						}
					}
					PvpProtocolTools.SetUnitState(units, info);
				}
				IL_179:
				if (units)
				{
					units.UpdateVisible();
					units.RefreshSyncState();
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		private static void ResetHeroStatistic(InBattleRuntimeInfo fightInfo)
		{
			if (fightInfo.unitCounters != null)
			{
				foreach (KeyValuePair<int, PlayerCounter> current in fightInfo.unitCounters)
				{
					PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(-current.Key);
					if (heroData != null)
					{
						PlayerCounter value = current.Value;
						heroData.HeroKill = value.killHoreCount;
						heroData.MonsterKill = value.killMonsterCount;
						heroData.Assist = value.helpKillHoreCount;
						heroData.FirstKill = value.isFirstBlood;
						heroData.Death = value.deadCount;
					}
					else
					{
						Debug.LogError("can't get hero with id: " + current.Key);
					}
				}
			}
			if (fightInfo.teamInfos != null && fightInfo.teamInfos.Length >= 2)
			{
				for (int i = 0; i < Mathf.Min(fightInfo.teamInfos.Length, 3); i++)
				{
					GroupTeamInfo groupTeamInfo = fightInfo.teamInfos[i];
					int teamType = i;
					if (i == 2)
					{
						teamType = 3;
					}
					PvpStatisticMgr.GroupData groupData = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData(teamType);
					groupData.TeamKill = groupTeamInfo.teamKill;
					groupData.TeamLv = groupTeamInfo.Level;
					groupData.TeamCurExp = groupTeamInfo.exp;
					groupData.TeamTowerDestroy = groupTeamInfo.killTowerCount;
					groupData.TeamEpicMonsterKill = groupTeamInfo.killSpeMonsterCount;
					groupData.TeamDeath = groupTeamInfo.deadCount;
				}
				if (fightInfo.teamInfos.Length > 3)
				{
					ClientLogger.Error("cannot handle it any more, too many teams");
				}
			}
			Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.UpdateView);
		}
	}
}
