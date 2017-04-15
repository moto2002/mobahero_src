using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros.AI
{
	internal static class CreepHelper
	{
		private enum EMonsterTriggerCondition
		{
			NormalMonster = 1,
			SummonedMonster,
			GoldMonster
		}

		public class CreepDeathData
		{
			public Monster Creep
			{
				get;
				set;
			}

			public Units Attacker
			{
				get;
				set;
			}

			public CreepDeathData(Monster creep, Units attacker)
			{
				this.Creep = creep;
				this.Attacker = attacker;
			}
		}

		private class CreepAwakeParam
		{
			public string text;

			public float time;

			public int bgLong;

			public CreepAwakeParam(string text, float time, int bgLong)
			{
				this.text = text;
				this.time = time;
				this.bgLong = bgLong;
			}
		}

		private const int CCreepUIPromptIdCreepBossKilled = 155;

		private const int CCreepUIPromptIdCreepBossUnderControl = 156;

		private const int CCreepUIPromptIdGoldKilled = 157;

		private const int CCreepUIPromptIdGoldPlunderedAttacker = 158;

		private const int CCreepUIPromptIdGoldPlunderedTarget = 159;

		private const int CCreepUIPromptIdAssistantCreep = 160;

		private const int CCreepUIPromptIdAssistantSoldier = 161;

		private const int CCreepUIPromptIdCampMe = 162;

		private const int CCreepUIPromptIdCampFriend = 163;

		private const int CCreepUIPromptIdCampEnemy = 164;

		private const int BUFF_TYPE_TEAM = 1;

		private const int BUFF_TYPE_SINGLE = 2;

		private static string BUFF_HP = "Buff_100";

		private static Dictionary<int, float> timeTables = new Dictionary<int, float>();

		private static Queue<CreepHelper.CreepAwakeParam> _waittingQueue = new Queue<CreepHelper.CreepAwakeParam>();

		private static float _lastBrocastTime = 0f;

		private static Task curTask = null;

		public static void AddHpPerSecond(Units target)
		{
			if (target.hp == target.hp_max)
			{
				return;
			}
			int unique_id = target.unique_id;
			if (!CreepHelper.timeTables.ContainsKey(unique_id))
			{
				CreepHelper.timeTables.Add(unique_id, Time.time);
			}
			else
			{
				float num = Time.time - CreepHelper.timeTables[unique_id];
				if (num < 1f)
				{
					return;
				}
				CreepHelper.timeTables[unique_id] = Time.time;
			}
			ActionManager.AddBuff(CreepHelper.BUFF_HP, target, target, true, string.Empty);
		}

		public static void AddBuff(string creepId, Units target)
		{
			if (!StringUtils.CheckValid(creepId))
			{
				return;
			}
			SysBattleMonsterCreepVo dataById = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(creepId);
			if (dataById != null && StringUtils.CheckValid(dataById.got_buff))
			{
				string[] stringValue = StringUtils.GetStringValue(dataById.got_buff, '|');
				int num = int.Parse(stringValue[0]);
				string buff_id = stringValue[1];
				if (num == 2)
				{
					ActionManager.AddBuff(buff_id, target, target, true, string.Empty);
				}
				else if (num == 1)
				{
					IList<Units> mapUnits = MapManager.Instance.GetMapUnits((TeamType)target.teamType, TargetTag.Hero);
					foreach (Units current in mapUnits)
					{
						ActionManager.AddBuff(buff_id, current, current, true, string.Empty);
					}
				}
			}
		}

		[DebuggerHidden]
		private static IEnumerator DelayBrocast()
		{
			return new CreepHelper.<DelayBrocast>c__Iterator27();
		}

		public static void ShowCreepInfo(CreepInfoType type, string arg, CreepHelper.CreepDeathData source = null)
		{
			string text = string.Empty;
			switch (type)
			{
			case CreepInfoType.creep_awake:
				if (arg != null)
				{
					SysPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPromptVo>("150");
					text = string.Format(LanguageManager.Instance.GetStringById(dataById.prompt_text), arg);
					if (dataById != null)
					{
						CreepHelper.CreepAwakeParam item = new CreepHelper.CreepAwakeParam(text, dataById.text_time, 629);
						CreepHelper._waittingQueue.Enqueue(item);
						if (CreepHelper.curTask == null)
						{
							CreepHelper.curTask = new Task(CreepHelper.DelayBrocast(), true);
						}
					}
				}
				break;
			case CreepInfoType.creep_killed:
				if (source != null && source.Attacker != null && source.Creep != null)
				{
					int num = (source.Attacker.teamType != 0) ? 151 : 152;
					SysPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPromptVo>(num.ToString());
					string stringById = LanguageManager.Instance.GetStringById(dataById.prompt_text);
					EntityType attackerType;
					if (source.Attacker.isBuilding)
					{
						attackerType = EntityType.Tower;
					}
					else if (source.Attacker.isMonster)
					{
						attackerType = EntityType.Monster;
					}
					else
					{
						attackerType = EntityType.Hero;
					}
					string sound = dataById.sound2;
					string promptId = PromptHelper.CreepKilledId(source.Attacker);
					UIMessageBox.ShowKillPrompt(promptId, "[]", source.Attacker.npc_id, attackerType, EntityType.Creep, string.Empty, string.Empty, TeamType.None, TeamType.None);
				}
				break;
			}
		}

		public static void ShowCreepWakePrompt(Monster monster)
		{
			string promptId = string.Empty;
			if (monster.npc_id.StartsWith("GoldStone"))
			{
				promptId = "1137";
			}
			if (monster.npc_id.StartsWith("DragonBoss"))
			{
				promptId = "1143";
			}
			if (monster.npc_id.StartsWith("DragonBossCall"))
			{
				promptId = "1149";
			}
			UIMessageBox.ShowTextPrompt(promptId);
		}

		public static string GetFxName(string id, CreepFxType type)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			int num = (int)type;
			SysBattleMonsterCreepVo dataById = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(id);
			if (dataById == null)
			{
				UnityEngine.Debug.LogError("no data get for id: " + id);
				return null;
			}
			string perform_id = dataById.perform_id;
			if (StringUtils.CheckValid(perform_id))
			{
				string[] stringValues = StringUtils.GetStringValues(perform_id, 0, ',', '|');
				int num2 = -1;
				for (int i = 0; i < stringValues.Length; i++)
				{
					if (stringValues[i].Equals(num.ToString()))
					{
						num2 = i;
						break;
					}
				}
				if (num2 == -1)
				{
					return null;
				}
				string[] stringValues2 = StringUtils.GetStringValues(perform_id, 1, ',', '|');
				if (stringValues2 != null && stringValues2.Length > num2)
				{
					return stringValues2[num2];
				}
			}
			return null;
		}

		private static CreepInfoType GetCreepInfoType(Units inCreep, string inBattleMonsterCreepId, int inOldGroupType)
		{
			if (!StringUtils.CheckValid(inBattleMonsterCreepId))
			{
				return CreepInfoType.creep_none;
			}
			SysBattleMonsterCreepVo dataById = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(inBattleMonsterCreepId);
			if (dataById == null)
			{
				return CreepInfoType.creep_none;
			}
			if (dataById.trigger_condition == 3)
			{
				if (inOldGroupType == 2)
				{
					return CreepInfoType.creep_gold_killed;
				}
				return CreepInfoType.creep_gold_plundered;
			}
			else if (StringUtils.CheckValid(dataById.summons2))
			{
				if (inOldGroupType == 2)
				{
					return CreepInfoType.creep_in_control;
				}
				return CreepInfoType.creep_killed;
			}
			else
			{
				if (StringUtils.CheckValid(dataById.summons))
				{
					return CreepInfoType.creep_spawn_assistantcreep;
				}
				if (StringUtils.CheckValid(dataById.assist_soldier))
				{
					return CreepInfoType.creep_spawn_assistantsoldier;
				}
				if (inCreep != null)
				{
					int data = inCreep.GetData<int>(DataType.ItemType);
					if (data == 128)
					{
						return CreepInfoType.creep_killed;
					}
				}
				return CreepInfoType.creep_none;
			}
		}

		private static string GetCreepUIMessageCampInfoContent(Units inAttacker)
		{
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				return PromptHelper.GetFriendlyText(inAttacker.TeamType);
			}
			int num;
			if (inAttacker.isPlayer)
			{
				num = 162;
			}
			else if (inAttacker.isMyTeam)
			{
				num = 163;
			}
			else
			{
				num = 164;
			}
			SysPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPromptVo>(num.ToString());
			if (dataById != null)
			{
				return LanguageManager.Instance.GetStringById(dataById.prompt_text);
			}
			return string.Empty;
		}

		public static void TryShowCreepUIMessageInfo(Units inAttacker, Units inCreep, string inBattleMonsterCreepId, int inOldGroupType)
		{
			if (inAttacker == null || inCreep == null)
			{
				return;
			}
			CreepInfoType creepInfoType = CreepHelper.GetCreepInfoType(inCreep, inBattleMonsterCreepId, inOldGroupType);
			if (creepInfoType == CreepInfoType.creep_none)
			{
				return;
			}
			string creepUIMessageCampInfoContent = CreepHelper.GetCreepUIMessageCampInfoContent(inAttacker);
			if (creepInfoType == CreepInfoType.creep_in_control)
			{
				string promptId = PromptHelper.CreepInControlId(inAttacker);
				UIMessageBox.ShowKillPrompt(promptId, inAttacker.npc_id, inCreep.npc_id, EntityType.Hero, EntityType.Creep, string.Empty, string.Empty, TeamType.None, TeamType.None);
			}
			else if (creepInfoType == CreepInfoType.creep_killed)
			{
				string promptId2 = PromptHelper.CreepKilledId(inAttacker);
				UIMessageBox.ShowKillPrompt(promptId2, inAttacker.npc_id, inCreep.npc_id, EntityType.Hero, EntityType.Creep, string.Empty, string.Empty, TeamType.None, TeamType.None);
			}
			else if (creepInfoType == CreepInfoType.creep_gold_killed)
			{
				if (inAttacker.isPlayer || inAttacker.isMyTeam)
				{
					string promptId3 = PromptHelper.CreepGoldKilledId(inAttacker);
					UIMessageBox.ShowKillPrompt(promptId3, inAttacker.npc_id, inCreep.npc_id, EntityType.Hero, EntityType.Creep, string.Empty, string.Empty, TeamType.None, TeamType.None);
				}
			}
			else if (creepInfoType == CreepInfoType.creep_gold_plundered)
			{
				string promptId4 = PromptHelper.CreepGoldPlunderedId(inAttacker);
				UIMessageBox.ShowKillPrompt(promptId4, inAttacker.npc_id, inCreep.npc_id, EntityType.Hero, EntityType.Creep, string.Empty, string.Empty, TeamType.None, TeamType.None);
			}
			else if (creepInfoType == CreepInfoType.creep_spawn_assistantcreep)
			{
				string promptId5 = PromptHelper.AssistantCreepKilledId(inAttacker);
				UIMessageBox.ShowKillPrompt(promptId5, inAttacker.npc_id, inCreep.npc_id, EntityType.Hero, EntityType.Creep, string.Empty, string.Empty, TeamType.None, TeamType.None);
			}
			else if (creepInfoType == CreepInfoType.creep_spawn_assistantsoldier)
			{
				string promptId6 = PromptHelper.SoldierCreepKilledId(inAttacker);
				UIMessageBox.ShowKillPrompt(promptId6, inAttacker.npc_id, inCreep.npc_id, EntityType.Hero, EntityType.Creep, string.Empty, string.Empty, TeamType.None, TeamType.None);
			}
		}

		public static void TryShowCreepSpecialEffect(string inBattleMonsterCreepId, Units inMonster)
		{
			if (inMonster == null)
			{
				return;
			}
			if (StringUtils.CheckValid(inBattleMonsterCreepId))
			{
				SysBattleMonsterCreepVo dataById = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(inBattleMonsterCreepId);
				string fxNameByBattleMonsterCreepData = CreepHelper.GetFxNameByBattleMonsterCreepData(dataById, CreepFxType.creep_under_control);
				if (StringUtils.CheckValid(fxNameByBattleMonsterCreepData))
				{
					ActionManager.PlayEffect(fxNameByBattleMonsterCreepData, inMonster, null, null, true, string.Empty, null);
				}
			}
		}

		public static string GetFxNameByBattleMonsterCreepData(SysBattleMonsterCreepVo inData, CreepFxType inType)
		{
			if (inData == null)
			{
				return "[]";
			}
			string perform_id = inData.perform_id;
			if (!StringUtils.CheckValid(perform_id))
			{
				return "[]";
			}
			int num = (int)inType;
			string[] array = perform_id.Split(new char[]
			{
				','
			});
			int num2 = array.Length;
			for (int i = 0; i < num2; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'|'
				});
				if (array2.Length == 2)
				{
					if (array2[0].Equals(num.ToString()))
					{
						return array2[1];
					}
				}
			}
			return "[]";
		}
	}
}
