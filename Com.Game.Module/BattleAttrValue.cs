using BattleAttrGrowth;
using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;

namespace Com.Game.Module
{
	internal class BattleAttrValue
	{
		public class RateData
		{
			public float normalRate;

			public float KillRate;

			public float firstKillRate;

			public float deathPenaltyRate;

			public float finishKillingRate;

			public float titleRate;

			public float killHeroRate;

			public RateData()
			{
				this.normalRate = 0f;
				this.KillRate = 0f;
				this.firstKillRate = 0f;
				this.deathPenaltyRate = 0f;
				this.finishKillingRate = 0f;
				this.titleRate = 0f;
				this.killHeroRate = 0f;
			}

			public void SetData(float normal, float kill, float firstSkill = 0f, float deathPenalty = 0f, float finishKilling = 0f)
			{
				this.normalRate = normal;
				this.KillRate = kill;
				this.firstKillRate = firstSkill;
				this.deathPenaltyRate = deathPenalty;
				this.finishKillingRate = finishKilling;
			}
		}

		public class BattleAttrConfigData
		{
			private enum ExpRateType
			{
				Normal,
				kill,
				Bonus,
				DeathPenalty,
				KillHero,
				Title
			}

			public BattleAttrValue.RateData monsterData;

			public BattleAttrValue.RateData towerData;

			public BattleAttrValue.RateData heroData;

			public BattleAttrConfigData()
			{
				this.monsterData = new BattleAttrValue.RateData();
				this.towerData = new BattleAttrValue.RateData();
				this.heroData = new BattleAttrValue.RateData();
			}

			private void SetValByType(BattleAttrValue.RateData hero, BattleAttrValue.RateData monster, BattleAttrValue.RateData tower, BattleAttrValue.BattleAttrConfigData.ExpRateType type, Dictionary<int, float> config)
			{
				switch (type)
				{
				case BattleAttrValue.BattleAttrConfigData.ExpRateType.Normal:
					hero.normalRate = config[1];
					monster.normalRate = config[2];
					tower.normalRate = config[3];
					break;
				case BattleAttrValue.BattleAttrConfigData.ExpRateType.kill:
					hero.KillRate = config[1];
					monster.KillRate = config[2];
					tower.KillRate = config[3];
					break;
				case BattleAttrValue.BattleAttrConfigData.ExpRateType.Bonus:
					hero.firstKillRate = config[2];
					monster.firstKillRate = config[1];
					hero.finishKillingRate = config[3];
					break;
				case BattleAttrValue.BattleAttrConfigData.ExpRateType.DeathPenalty:
					hero.deathPenaltyRate = config[1];
					monster.deathPenaltyRate = config[2];
					tower.deathPenaltyRate = config[3];
					break;
				}
			}

			private Dictionary<int, float> GetArrayFromConfig(string val)
			{
				Dictionary<int, float> dictionary = new Dictionary<int, float>();
				string[] array = val.Split(new char[]
				{
					','
				});
				if (array != null)
				{
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text = array2[i];
						string[] array3 = text.Split(new char[]
						{
							'|'
						});
						if (array3 != null)
						{
							int key = int.Parse(array3[0]);
							float value = float.Parse(array3[1]);
							dictionary[key] = value;
						}
					}
				}
				return dictionary;
			}
		}

		private float _lastExpRatio;

		private int _lastTeamLevel;

		private float _normalExp;

		private float _killExp;

		private float _bonusExp;

		private float _deathExp;

		private float _totalExp;

		private float _curRicherExp;

		private int MAX_LEVEL = 18;

		private static Dictionary<int, float> _expDics;

		private static BattleAttrValue.BattleAttrConfigData _configData;

		private static bool _hasInit;

		private TeamType _teamType;

		private int _teamLevel;

		private float _expRatio;

		private bool isPlayer;

		public TeamType EnemyTeam
		{
			get
			{
				return (!this.isPlayer) ? TeamType.LM : TeamType.BL;
			}
		}

		public TeamType Team
		{
			get
			{
				return this._teamType;
			}
		}

		public int TeamLevel
		{
			get
			{
				return this._teamLevel;
			}
		}

		public float ExpRatio
		{
			get
			{
				return this._expRatio;
			}
		}

		public float NormalExp
		{
			get
			{
				return this._normalExp;
			}
		}

		public float KillExp
		{
			get
			{
				return this._killExp;
			}
		}

		public float BonusExp
		{
			get
			{
				return this._bonusExp;
			}
		}

		public float DeathExp
		{
			get
			{
				return this._deathExp;
			}
		}

		public float TotalExp
		{
			get
			{
				return this._totalExp;
			}
		}

		public MyStatistic.MyStatisticData MyData
		{
			get
			{
				return MyStatistic.Instance.GetDataByTeam(this.Team);
			}
		}

		public MyStatistic.MyStatisticData EnemyData
		{
			get
			{
				return MyStatistic.Instance.GetDataByTeam(this.EnemyTeam);
			}
		}

		public BattleAttrValue.BattleAttrConfigData ConfigData
		{
			get
			{
				return BattleAttrValue._configData;
			}
		}

		public BattleAttrValue(TeamType type)
		{
			this._teamType = type;
			this.Init();
		}

		public float GetHeroContribution(Units target)
		{
			int unique_id = target.unique_id;
			if (this.MyData == null)
			{
				return 0f;
			}
			int num = this.MyData.heroKill.GetHeroRecord(unique_id);
			int num2 = this.MyData.monsterKill.GetHeroRecord(unique_id);
			int heroRecord = this.MyData.towerKill.GetHeroRecord(unique_id);
			AchieveData achieveData = GameManager.Instance.AchieveManager.GetAchieveData(unique_id, target.teamType);
			if (achieveData != null)
			{
				num = achieveData.TotalKill;
				num2 = achieveData.MonsterKillNum;
			}
			float num3 = (float)num2 * this.ConfigData.monsterData.normalRate + (float)num * this.ConfigData.heroData.normalRate + (float)heroRecord * this.ConfigData.towerData.normalRate;
			float num4 = (float)num2 * this.ConfigData.monsterData.KillRate + (float)heroRecord * this.ConfigData.towerData.KillRate + (float)num * this.GetKillHeroSingleReward();
			BattleAttrGrowth.KillData killData = GameManager.Instance.AchieveManager.GetKillData(this.Team);
			int num5 = (killData.KillFirstHeroId != unique_id) ? 0 : 1;
			int num6 = (killData.KillFirstMonsterId != unique_id) ? 0 : 1;
			float num7 = (float)num5 * this.ConfigData.heroData.firstKillRate + (float)num6 * this.ConfigData.monsterData.firstKillRate;
			float num8 = 0f;
			if (achieveData != null)
			{
				num8 = (float)(-(float)achieveData.SelfDeathTime) * this.ConfigData.heroData.deathPenaltyRate;
			}
			return num3 + num4 + num7 + num8;
		}

		private string GetTeamDes()
		{
			if (this.Team == TeamType.LM)
			{
				return "LM";
			}
			return "BL";
		}

		private void CaclNormalExp()
		{
			this._normalExp = (float)this.MyData.monsterKill.TotalKill * this.ConfigData.monsterData.normalRate + (float)this.MyData.heroKill.TotalKill * this.ConfigData.heroData.normalRate + (float)this.MyData.towerKill.TotalKill * this.ConfigData.towerData.normalRate;
		}

		private void CaclKillExp()
		{
			this._killExp = (float)this.MyData.monsterKill.byHero * this.ConfigData.monsterData.KillRate + (float)this.MyData.towerKill.byHero * this.ConfigData.towerData.KillRate + (float)this.MyData.heroKill.byHero * this.GetKillHeroSingleReward();
		}

		private void CaclBonusExp()
		{
			BattleAttrGrowth.KillData killData = GameManager.Instance.AchieveManager.GetKillData(this.Team);
			int num = (!killData.KillFirstHero) ? 0 : 1;
			int num2 = (!killData.KillFirstMonster) ? 0 : 1;
			this._bonusExp = (float)num * this.ConfigData.heroData.firstKillRate + (float)num2 * this.ConfigData.monsterData.firstKillRate + (float)killData.FinishKillingTimes * this.ConfigData.heroData.finishKillingRate;
		}

		private void CaclDeathExp()
		{
			this._deathExp = (float)(-(float)this.EnemyData.heroKill.TotalKill) * this.ConfigData.heroData.deathPenaltyRate;
		}

		private void CaclTotalExp()
		{
			this._totalExp = this._normalExp + this._killExp + this._bonusExp + this._deathExp;
			if (this._totalExp <= 0f)
			{
				this._totalExp = 0f;
			}
		}

		private float GetKillHeroSingleReward()
		{
			int teamLevel = BattleAttrManager.Instance.GetTeamLevel(this.EnemyTeam);
			return this.ConfigData.heroData.KillRate + this.ConfigData.heroData.killHeroRate * (float)(teamLevel - 1) + (float)GameManager.Instance.AchieveManager.GetKillData(this.Team).TitleLevel * this.ConfigData.heroData.titleRate;
		}

		private void CheckChange()
		{
			int num = this._teamLevel - this._lastTeamLevel;
			float num2 = this._expRatio - this._lastExpRatio;
			if (num != 0 || num2 != 0f)
			{
				this.DoAnimation(num, this._lastTeamLevel, this._lastExpRatio, this._expRatio);
			}
			this._lastExpRatio = this._expRatio;
			this._lastTeamLevel = this._teamLevel;
		}

		private void CheckLevel()
		{
			if (this._teamLevel > this.MAX_LEVEL - 1)
			{
				return;
			}
			if (!BattleAttrValue._expDics.ContainsKey(this._teamLevel))
			{
				return;
			}
			float totalExpByLevel = this.GetTotalExpByLevel(this._teamLevel);
			this._curRicherExp = this._totalExp - totalExpByLevel;
			if (this._curRicherExp < 0f)
			{
				this.LevelDown();
				return;
			}
			if (this._curRicherExp >= BattleAttrValue._expDics[this._teamLevel])
			{
				this.LevelUp();
				return;
			}
		}

		private float GetTotalExpByLevel(int level)
		{
			if (!BattleAttrValue._expDics.ContainsKey(level) || level > this.MAX_LEVEL)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 1; i < level; i++)
			{
				num += BattleAttrValue._expDics[i];
			}
			return num;
		}

		private void LevelUp()
		{
			if (!BattleAttrValue._expDics.ContainsKey(this._teamLevel + 1))
			{
				return;
			}
			this._teamLevel++;
			this._curRicherExp = this._totalExp - this.GetTotalExpByLevel(this._teamLevel);
			if (this._teamLevel < this.MAX_LEVEL && this._curRicherExp >= BattleAttrValue._expDics[this._teamLevel])
			{
				this.LevelUp();
			}
			else if (this._teamLevel == this.MAX_LEVEL)
			{
				this._curRicherExp = 0f;
			}
		}

		private void LevelDown()
		{
			if (this._teamLevel == 1)
			{
				return;
			}
			this._teamLevel--;
			this._curRicherExp = this._totalExp - this.GetTotalExpByLevel(this._teamLevel);
			if (this._curRicherExp < 0f)
			{
				this.LevelDown();
			}
		}

		private void CaclCurExpRatio()
		{
			this._expRatio = this._curRicherExp / BattleAttrValue._expDics[this._teamLevel];
		}

		public void DoAnimation(int levelChange, int orginLevel, float originRate, float curRate)
		{
			AttachedPropertyMediator mediator = Singleton<AttachedPropertyView>.Instance.Mediator;
			if (mediator != null)
			{
				mediator.StartAnimateDate(levelChange, orginLevel, originRate, curRate, this.isPlayer, this.MAX_LEVEL);
			}
		}

		public void SetKill(int kill)
		{
			AttachedPropertyMediator mediator = Singleton<AttachedPropertyView>.Instance.Mediator;
			if (mediator != null)
			{
				mediator.SetTeamKill(this.isPlayer, kill);
			}
		}

		private void Init()
		{
			this.InitConfigData();
			this.ResetData();
			if (this._teamType == TeamType.LM)
			{
				this.isPlayer = true;
			}
			else
			{
				this.isPlayer = false;
			}
		}

		public void Update()
		{
		}

		public void End()
		{
			this.ResetData();
		}

		public void UpdateData()
		{
			if (this.MyData == null)
			{
				return;
			}
			this.CaclNormalExp();
			this.CaclKillExp();
			this.CaclBonusExp();
			this.CaclDeathExp();
			this.CaclTotalExp();
			this.CheckLevel();
			this.CaclCurExpRatio();
			this.CheckChange();
		}

		private void InitConfigData()
		{
			this.InitExpDic();
			BattleAttrValue._configData = new BattleAttrValue.BattleAttrConfigData();
		}

		private void InitExpDic()
		{
			Dictionary<string, object>.ValueCollection values = BaseDataMgr.instance.GetDicByType<SysBattleAttrLvVo>().Values;
			this.MAX_LEVEL = values.Count;
			BattleAttrValue._expDics = new Dictionary<int, float>();
			for (int i = 1; i <= this.MAX_LEVEL; i++)
			{
				SysBattleAttrLvVo dataById = BaseDataMgr.instance.GetDataById<SysBattleAttrLvVo>(i.ToString());
				if (dataById != null)
				{
					BattleAttrValue._expDics[i] = dataById.exp_to_next;
				}
			}
		}

		public void ResetData()
		{
			this._bonusExp = 0f;
			this._deathExp = 0f;
			this._killExp = 0f;
			this._totalExp = 0f;
			this._teamLevel = 1;
			this._bonusExp = 0f;
			this._expRatio = 0f;
			this._curRicherExp = 0f;
			this._lastExpRatio = 0f;
			this._lastTeamLevel = 1;
		}
	}
}
