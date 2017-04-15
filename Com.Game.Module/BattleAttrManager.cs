using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros.Pvp;
using MobaHeros.Spawners;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	internal class BattleAttrManager
	{
		private const float TEAM_DAMAGE_MODIFIER = 8.5f;

		public static bool isBattleAttrOpen = true;

		private int levelType = 1;

		private static BattleAttrManager _instance;

		private BattleAttrValue _BLAttrValue;

		private BattleAttrValue _LMAttrValue;

		private Dictionary<int, bool> _blUnlockSkills;

		private Dictionary<int, bool> _lmUnlockSkills;

		private Dictionary<int, int> unlockSkillConfigs;

		private Dictionary<int, float> _playerDeathTime = new Dictionary<int, float>();

		private Dictionary<int, float> _playerDeathLastTime = new Dictionary<int, float>();

		private float lastUpdateTime;

		private float interval = 0.5f;

		public static BattleAttrManager Instance
		{
			get
			{
				if (BattleAttrManager._instance == null)
				{
					BattleAttrManager._instance = new BattleAttrManager();
				}
				return BattleAttrManager._instance;
			}
		}

		public BattleAttrValue BLAttrValue
		{
			get
			{
				return this._BLAttrValue;
			}
		}

		public BattleAttrValue LMAttrVale
		{
			get
			{
				return this._LMAttrValue;
			}
		}

		public BattleAttrManager()
		{
			this._LMAttrValue = new BattleAttrValue(TeamType.LM);
			this._BLAttrValue = new BattleAttrValue(TeamType.BL);
			this.InitUnlockSkillData();
		}

		public float GetHeroContribution(Units target)
		{
			if (target.teamType == 0)
			{
				return this.LMAttrVale.GetHeroContribution(target);
			}
			return this.BLAttrValue.GetHeroContribution(target);
		}

		public void SetPlayerDeathTimer(int id, float time)
		{
			if (this.lastUpdateTime == 0f)
			{
				this.lastUpdateTime = Time.time;
			}
			else
			{
				this.UpdateUnitsDeathTime();
				this.lastUpdateTime = Time.time;
			}
			if (this._playerDeathTime.ContainsKey(id))
			{
				this._playerDeathTime[id] = time;
			}
			else
			{
				this._playerDeathTime.Add(id, time);
			}
			if (this._playerDeathLastTime.ContainsKey(id))
			{
				this._playerDeathLastTime[id] = time;
			}
			else
			{
				this._playerDeathLastTime.Add(id, time);
			}
		}

		public void ClearDeathLastTime()
		{
			this.lastUpdateTime = 0f;
			this._playerDeathLastTime.Clear();
		}

		public float GetPlayerDeathLastTime(int id)
		{
			this.UpdateUnitsDeathTime();
			if (this._playerDeathLastTime.ContainsKey(id))
			{
				return this._playerDeathLastTime[id];
			}
			return 0f;
		}

		public float GetPlayerDeathTime(int id)
		{
			if (this._playerDeathTime.ContainsKey(id))
			{
				return this._playerDeathTime[id];
			}
			return -1f;
		}

		private void InitUnlockSkillData()
		{
			this._blUnlockSkills = new Dictionary<int, bool>();
			this._lmUnlockSkills = new Dictionary<int, bool>();
		}

		private Dictionary<int, int> GetArrayFromConfig(string val)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			string[] stringValue = StringUtils.GetStringValue(val, ',');
			if (stringValue != null)
			{
				string[] array = stringValue;
				for (int i = 0; i < array.Length; i++)
				{
					string str = array[i];
					if (StringUtils.CheckValid(str))
					{
						string[] stringValue2 = StringUtils.GetStringValue(str, '|');
						if (stringValue2 != null)
						{
							int key = int.Parse(stringValue2[0]);
							int value = int.Parse(stringValue2[1]);
							dictionary[key] = value;
						}
					}
				}
			}
			return dictionary;
		}

		private TeamType GetEnemyTeam(Units target)
		{
			if (target.teamType == 0)
			{
				return TeamType.BL;
			}
			return TeamType.LM;
		}

		public bool IsCurSkillUnlock(int skillIdx, Units target)
		{
			if (GlobalSettings.isSkillUnlock || (LevelManager.IsTestingLevel && !LevelManager.Instance.CheckSceneIsTest()))
			{
				return true;
			}
			if (!SceneInfo.Current.IsOpenAdditionFactor)
			{
				return true;
			}
			if (skillIdx >= 4)
			{
				return true;
			}
			if (target.skillManager.skills_index.ContainsKey(skillIdx))
			{
				string skillID = target.skillManager.skills_index[skillIdx];
				return target.skillManager.IsSkillUnlock(skillID);
			}
			return false;
		}

		public bool IsCurSkillCanAdd(int skillIdx, Units target, int skillLevel = -1, int skillMaxLevel = -1)
		{
			if (GlobalSettings.isSkillUnlock || (LevelManager.IsTestingLevel && !LevelManager.Instance.CheckSceneIsTest()))
			{
				return true;
			}
			if (!SceneInfo.Current.IsOpenAdditionFactor)
			{
				return true;
			}
			if (target == null)
			{
				return false;
			}
			TeamType teamType = (TeamType)((!target.MeiHuo.IsInState) ? target.teamType : ((int)this.GetEnemyTeam(target)));
			if (teamType == TeamType.BL)
			{
				Dictionary<int, bool> dictionary = this._blUnlockSkills;
			}
			else
			{
				Dictionary<int, bool> dictionary = this._lmUnlockSkills;
			}
			int num;
			if (this.levelType == 2)
			{
				num = this.GetTeamLevel(teamType);
			}
			else
			{
				num = UtilManager.Instance.GetHerolv(target.unique_id);
			}
			if (skillMaxLevel == 0 || skillLevel == skillMaxLevel)
			{
				return false;
			}
			int num2;
			if (skillLevel >= 0)
			{
				string text = string.Empty;
				string unikey = "1";
				SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
				if (dataById != null)
				{
					unikey = dataById.skill_limit;
				}
				switch (skillIdx)
				{
				case 0:
					text = BaseDataMgr.instance.GetDataById<SysSkillLevelupLimitVo>(unikey).QLevelLimit;
					break;
				case 1:
					text = BaseDataMgr.instance.GetDataById<SysSkillLevelupLimitVo>(unikey).WLevelLimit;
					break;
				case 2:
					text = BaseDataMgr.instance.GetDataById<SysSkillLevelupLimitVo>(unikey).ELevelLimit;
					break;
				case 3:
					text = BaseDataMgr.instance.GetDataById<SysSkillLevelupLimitVo>(unikey).RLevelLimit;
					break;
				}
				string[] array = text.Split(new char[]
				{
					','
				});
				num2 = int.Parse(array[skillLevel]);
			}
			else
			{
				num2 = this.unlockSkillConfigs[skillIdx];
			}
			return num >= num2 && skillLevel < skillMaxLevel && skillMaxLevel > 0;
		}

		public void UpdateSkillView()
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (!player)
			{
				return;
			}
			for (int i = 0; i < player.skillManager.GetSkills().Count; i++)
			{
				if (!this.IsCurSkillUnlock(i, player))
				{
					break;
				}
				Singleton<SkillView>.Instance.UpdateSkillByIndex(i);
			}
		}

		public void Init()
		{
			MyStatistic.Instance.Init();
			this.End();
		}

		public void Update()
		{
			if (!SceneInfo.Current.IsOpenAdditionFactor || LevelManager.Instance.IsPvpBattleType)
			{
				return;
			}
			if (this._LMAttrValue != null)
			{
				this._LMAttrValue.UpdateData();
			}
			if (this._BLAttrValue != null)
			{
				this._BLAttrValue.UpdateData();
			}
		}

		private void UpdateUnitsDeathTime()
		{
			if (this._playerDeathLastTime.Count > 0)
			{
				List<int> list = new List<int>(this._playerDeathLastTime.Keys);
				foreach (int current in list)
				{
					Dictionary<int, float> playerDeathLastTime;
					Dictionary<int, float> expr_3C = playerDeathLastTime = this._playerDeathLastTime;
					int key;
					int expr_3F = key = current;
					float num = playerDeathLastTime[key];
					expr_3C[expr_3F] = num - (Time.time - this.lastUpdateTime);
					if (this._playerDeathLastTime[current] <= 0f)
					{
						this._playerDeathLastTime.Remove(current);
					}
				}
				this.lastUpdateTime = Time.time;
			}
		}

		public void End()
		{
			this._LMAttrValue.End();
			this._BLAttrValue.End();
			this._lmUnlockSkills.Clear();
			this._blUnlockSkills.Clear();
			this._playerDeathTime.Clear();
		}

		public float AdjustFinalDamage(Units self, Units target, float originVal)
		{
			float num;
			if (target.isHero)
			{
				num = this.TargetIsHeroDamage(self, target);
			}
			else
			{
				num = this.TargetIsNotHeroDamage(self, target);
			}
			return originVal * num;
		}

		private float TargetIsHeroDamage(Units caster, Units target)
		{
			int teamLevel = this.GetTeamLevel((TeamType)caster.teamType);
			int teamLevel2 = this.GetTeamLevel((TeamType)target.teamType);
			return ((float)(teamLevel - 1) + 8.5f) / ((float)(teamLevel2 - 1) + 8.5f);
		}

		private float TargetIsNotHeroDamage(Units caster, Units target)
		{
			int teamLevel = this.GetTeamLevel((TeamType)caster.teamType);
			return ((float)(teamLevel - 1) + 8.5f) / 8.5f;
		}

		public int GetTeamLevel(TeamType type)
		{
			if (LevelManager.Instance.IsPvpBattleType)
			{
				PvpStatisticMgr.GroupData groupData = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData((int)type);
				return groupData.TeamLv;
			}
			if (type == TeamType.LM)
			{
				return this._LMAttrValue.TeamLevel;
			}
			return this._BLAttrValue.TeamLevel;
		}
	}
}
