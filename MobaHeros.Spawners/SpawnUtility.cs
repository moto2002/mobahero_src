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
using System.Linq;
using UnityEngine;
using Utils;

namespace MobaHeros.Spawners
{
	public class SpawnUtility
	{
		private readonly SysBattleSceneVo _myScene;

		public SpawnUtility(SysBattleSceneVo myScene)
		{
			this._myScene = myScene;
		}

		public static float GetAttrFactor(TeamType teamType, SysBattleSceneVo myScene)
		{
			float num = 1f;
			if (teamType == TeamType.BL)
			{
				num = myScene.hero2_factor;
			}
			if (num <= 1.401298E-45f)
			{
				ClientLogger.Warn("attrFactor is zero, ignore");
				num = 1f;
			}
			return num;
		}

		public float GetAttrFactor(TeamType teamType)
		{
			return SpawnUtility.GetAttrFactor(teamType, this._myScene);
		}

		public Units SpawnInstance(EntityVo npcinfo, string tag, TeamType teamType, int spawnPos, string respawnInterval = "[]", Transform newSpawnPoint = null, UnitControlType controlType = UnitControlType.None, UnitType unitType = UnitType.None)
		{
			if (npcinfo == null || npcinfo.npc_id == null || npcinfo.npc_id == string.Empty || tag == null)
			{
				ClientLogger.Error("SpawnInstance line 1 null");
				return null;
			}
			Dictionary<AttrType, float> dictionary = new Dictionary<AttrType, float>();
			if (tag == "Hero" || tag == "Player")
			{
				string npc_id = npcinfo.npc_id;
				int level = npcinfo.level;
				int quality = npcinfo.quality;
				int star = npcinfo.star;
				float hp = npcinfo.hp;
				float mp = npcinfo.mp;
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(npc_id);
				if (heroMainData == null || heroMainData.model_id == null)
				{
					ClientLogger.Error("SpawnInstance: no hero found #" + npc_id);
					return null;
				}
				Dictionary<DataType, object> dictionary2 = new Dictionary<DataType, object>
				{
					{
						DataType.NameId,
						npc_id
					},
					{
						DataType.ModelId,
						heroMainData.model_id
					},
					{
						DataType.TeamType,
						teamType
					},
					{
						DataType.AIType,
						2
					},
					{
						DataType.AttrFactor,
						SpawnUtility.GetAttrFactor(teamType, this._myScene)
					},
					{
						DataType.Level,
						level
					},
					{
						DataType.Quality,
						quality
					},
					{
						DataType.Star,
						star
					},
					{
						DataType.Skin,
						npcinfo.skin
					}
				};
				if (npcinfo.uid != 0)
				{
					dictionary2.Add(DataType.UniqueId, npcinfo.uid);
				}
				if (hp != 0f)
				{
					dictionary.Add(AttrType.Hp, hp);
				}
				if (mp != 0f)
				{
					dictionary.Add(AttrType.Mp, mp);
				}
				Transform transform = newSpawnPoint;
				if (!transform)
				{
					transform = MapManager.Instance.GetSpawnPos(teamType, spawnPos);
				}
				Units result = MapManager.Instance.SpawnUnit(tag, dictionary2, dictionary, transform.position, transform.rotation, controlType, true, null, unitType);
				if (!transform)
				{
					ClientLogger.Error(string.Format("SpawnInstance： GetSpawnPos failed for {0} {1} in {2}", teamType, spawnPos, LevelManager.CurLevelId));
				}
				return result;
			}
			else if (tag.Equals("Home"))
			{
				string npc_id2 = npcinfo.npc_id;
				Transform transform2 = newSpawnPoint;
				if (!transform2)
				{
					transform2 = MapManager.Instance.GetSpawnPos(teamType, npcinfo.pos);
				}
				SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(npc_id2);
				if (monsterMainData == null)
				{
					ClientLogger.Error("SpawnInstance: no home found #" + npc_id2);
					return null;
				}
				Dictionary<DataType, object> dictionary3 = new Dictionary<DataType, object>
				{
					{
						DataType.NameId,
						npc_id2
					},
					{
						DataType.ModelId,
						monsterMainData.model_id
					},
					{
						DataType.TeamType,
						teamType
					},
					{
						DataType.AIType,
						npcinfo.ai_type
					},
					{
						DataType.AttrFactor,
						SpawnUtility.GetAttrFactor(teamType, this._myScene)
					},
					{
						DataType.Skin,
						npcinfo.skin
					}
				};
				if (npcinfo.uid != 0)
				{
					dictionary3.Add(DataType.UniqueId, npcinfo.uid);
				}
				return MapManager.Instance.SpawnUnit(tag, dictionary3, dictionary, transform2.position, transform2.rotation, controlType, false, null, unitType);
			}
			else
			{
				string npc_id3 = npcinfo.npc_id;
				Transform transform3 = newSpawnPoint;
				if (!transform3)
				{
					transform3 = MapManager.Instance.GetSpawnPos(teamType, npcinfo.pos);
				}
				SysMonsterMainVo monsterMainData2 = BaseDataMgr.instance.GetMonsterMainData(npc_id3);
				if (monsterMainData2 == null)
				{
					ClientLogger.Error("SpawnInstance: no npc found #" + npc_id3);
					return null;
				}
				Dictionary<DataType, object> dictionary4 = new Dictionary<DataType, object>
				{
					{
						DataType.NameId,
						npc_id3
					},
					{
						DataType.ModelId,
						monsterMainData2.model_id
					},
					{
						DataType.TeamType,
						teamType
					},
					{
						DataType.AIType,
						npcinfo.ai_type
					},
					{
						DataType.AttrFactor,
						SpawnUtility.GetAttrFactor(teamType, this._myScene)
					},
					{
						DataType.Skin,
						npcinfo.skin
					}
				};
				if (npcinfo.uid != 0)
				{
					dictionary4.Add(DataType.UniqueId, npcinfo.uid);
				}
				return MapManager.Instance.SpawnUnit(tag, dictionary4, dictionary, transform3.position, transform3.rotation, controlType, false, null, unitType);
			}
		}

		public Units SpawnEyeItemInstance(EntityVo npcinfo, string tag, TeamType teamType, string respawnInterval, Transform newSpawnPoint, SVector3 eyeItemInfoInst, string eyeItemPreObjRes, UnitControlType controlType)
		{
			if (npcinfo == null || npcinfo.npc_id == null || npcinfo.npc_id == string.Empty || tag == null)
			{
				return null;
			}
			string npc_id = npcinfo.npc_id;
			SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(npc_id);
			if (monsterMainData == null)
			{
				ClientLogger.Error("SpawnInstance: no npc found #" + npc_id);
				return null;
			}
			Dictionary<DataType, object> dictionary = new Dictionary<DataType, object>
			{
				{
					DataType.NameId,
					npc_id
				},
				{
					DataType.ModelId,
					monsterMainData.model_id
				},
				{
					DataType.TeamType,
					teamType
				},
				{
					DataType.AIType,
					npcinfo.ai_type
				},
				{
					DataType.AttrFactor,
					SpawnUtility.GetAttrFactor(teamType, this._myScene)
				}
			};
			if (npcinfo.uid != 0)
			{
				dictionary.Add(DataType.UniqueId, npcinfo.uid);
			}
			Dictionary<AttrType, float> unitAttrs = new Dictionary<AttrType, float>();
			return MapManager.Instance.SpawnEyeItemUnit(tag, dictionary, unitAttrs, newSpawnPoint, eyeItemInfoInst, eyeItemPreObjRes, controlType, false);
		}

		public Units SpawnPvpHero(EntityVo npcinfo, string tag, TeamType teamType, int spawnPos, string userName = "", string summerId = "", Vector3? specifyPos = null, UnitType unitType = UnitType.None)
		{
			if (npcinfo == null || npcinfo.npc_id == string.Empty)
			{
				return null;
			}
			if (tag != "Hero" && tag != "Player")
			{
				return null;
			}
			string npc_id = npcinfo.npc_id;
			Dictionary<DataType, object> dictionary = new Dictionary<DataType, object>();
			int num = 2;
			int level = npcinfo.level;
			int quality = npcinfo.quality;
			int star = npcinfo.star;
			float hp = npcinfo.hp;
			float mp = npcinfo.mp;
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(npc_id);
			dictionary.Add(DataType.NameId, npc_id);
			dictionary.Add(DataType.ModelId, heroMainData.model_id);
			dictionary.Add(DataType.TeamType, teamType);
			dictionary.Add(DataType.AIType, num);
			dictionary.Add(DataType.AttrFactor, this.GetAttrFactor(teamType));
			dictionary.Add(DataType.Level, level);
			dictionary.Add(DataType.Quality, quality);
			dictionary.Add(DataType.Star, star);
			if (npcinfo.uid != 0)
			{
				dictionary.Add(DataType.UniqueId, npcinfo.uid);
			}
			if (npcinfo.effectId != "Default")
			{
				dictionary.Add(DataType.EffectId, npcinfo.effectId);
			}
			Dictionary<AttrType, float> dictionary2 = new Dictionary<AttrType, float>();
			if (hp != 0f)
			{
				dictionary2.Add(AttrType.Hp, hp);
			}
			if (mp != 0f)
			{
				dictionary2.Add(AttrType.Mp, mp);
			}
			bool reuse = false;
			if (LevelManager.Instance.IsPvpBattleType)
			{
				reuse = true;
				if (userName == string.Empty)
				{
					dictionary.Add(DataType.summerName, Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo obj) => obj.newUid.ToString() == Math.Abs(npcinfo.uid).ToString()).userName);
				}
				else
				{
					dictionary.Add(DataType.summerName, userName);
				}
				if (summerId == string.Empty)
				{
					dictionary.Add(DataType.summerId, Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo obj) => obj.newUid.ToString() == Math.Abs(npcinfo.uid).ToString()).SummerId);
				}
				else
				{
					dictionary.Add(DataType.summerId, summerId);
				}
			}
			else if (LevelManager.Instance.IsServerZyBattleType)
			{
				reuse = true;
			}
			Vector3 position = Vector3.zero;
			if (!specifyPos.HasValue)
			{
				position = MapManager.Instance.GetSpawnPos(teamType, spawnPos).position;
			}
			else
			{
				position = specifyPos.Value;
			}
			return MapManager.Instance.SpawnUnit(tag, dictionary, dictionary2, position, Quaternion.identity, UnitControlType.None, reuse, null, unitType);
		}

		public Units RespawnUnit(Units target)
		{
			if (target == null)
			{
				return null;
			}
			Dictionary<DataType, object> unitData = new Dictionary<DataType, object>(default(EnumEqualityComparer<DataType>))
			{
				{
					DataType.UniqueId,
					target.unique_id
				},
				{
					DataType.NameId,
					target.npc_id
				},
				{
					DataType.ModelId,
					target.model_id
				},
				{
					DataType.TeamType,
					target.teamType
				},
				{
					DataType.AIType,
					target.aiType
				},
				{
					DataType.AttrFactor,
					this.GetAttrFactor((TeamType)target.teamType)
				},
				{
					DataType.Level,
					target.level
				},
				{
					DataType.Quality,
					target.quality
				},
				{
					DataType.Star,
					target.star
				}
			};
			Vector3 spwan_pos = target.spwan_pos;
			Quaternion spwan_rotation = target.spwan_rotation;
			int skillPointsLeft = target.skillManager.SkillPointsLeft;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			string[] skillIDs = target.skillManager.GetSkillIDs();
			string[] array = skillIDs;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				dictionary.Add(text, target.skillManager.GetSkillLevel(text));
			}
			Units units = MapManager.Instance.RespawnUnit(target.unique_id, target.tag, unitData, spwan_pos, spwan_rotation, skillPointsLeft, dictionary, UnitControlType.None);
			if (units != null && units.moveController != null)
			{
				units.moveController.ResetMoveInfosOnRelive();
			}
			target.playVoice("onReborn");
			units.Rebirth();
			return units;
		}

		[DebuggerHidden]
		public IEnumerator RespawnPveHero_Coroutinue(Units target, float spawnInterval = 0f, float spawnDelay = 0f)
		{
			SpawnUtility.<RespawnPveHero_Coroutinue>c__Iterator1CE <RespawnPveHero_Coroutinue>c__Iterator1CE = new SpawnUtility.<RespawnPveHero_Coroutinue>c__Iterator1CE();
			<RespawnPveHero_Coroutinue>c__Iterator1CE.target = target;
			<RespawnPveHero_Coroutinue>c__Iterator1CE.spawnInterval = spawnInterval;
			<RespawnPveHero_Coroutinue>c__Iterator1CE.spawnDelay = spawnDelay;
			<RespawnPveHero_Coroutinue>c__Iterator1CE.<$>target = target;
			<RespawnPveHero_Coroutinue>c__Iterator1CE.<$>spawnInterval = spawnInterval;
			<RespawnPveHero_Coroutinue>c__Iterator1CE.<$>spawnDelay = spawnDelay;
			<RespawnPveHero_Coroutinue>c__Iterator1CE.<>f__this = this;
			return <RespawnPveHero_Coroutinue>c__Iterator1CE;
		}

		private void SetPlayerOnRelive(Units target)
		{
			TeamType myTeam = TeamManager.MyTeam;
			if (null == target)
			{
				return;
			}
			if (target.teamType != (int)myTeam)
			{
				return;
			}
			if (target.tag == "Monster")
			{
				return;
			}
			if (BattleCameraMgr.Instance.WatchTarget == target)
			{
				PlayerControlMgr.Instance.ChangePlayer(target, true);
				target.MarkAsMainPlayer(true);
				BattleCameraMgr.Instance.PlayerRespawn();
			}
			if (target.isPlayer)
			{
				StrategyManager.Instance.UpdateInputState(true);
			}
			bool flag = true;
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits(myTeam, global::TargetTag.Player);
			if (mapUnits != null)
			{
				for (int i = 0; i < mapUnits.Count; i++)
				{
					if (target != mapUnits[i] && mapUnits[i].isLive)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
			}
		}

		public void RespawnPvpHero(Units target)
		{
			this.RespawnUnit(target);
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitRebirthAgain, target, null, null);
			this.SetPlayerOnRelive(target);
		}
	}
}
