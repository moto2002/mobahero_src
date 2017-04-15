using System;
using System.Collections.Generic;
using UnityEngine;

public class Strategy
{
	private class FactorWeigh
	{
		public float timeInterval;

		public float timeWeigh;

		public float leagueSurviveHeroWeigh;

		public float leagueSurviveMonsterWeigh;

		public float leagueSkillCoolWeigh;

		public float leagueLeftManaWeigh;

		public float leagueAverageHpWeigh;

		public float EnemySurviveHeroWeigh;

		public float EnemySurviveMonsterWeigh;

		public float EnemySkillCoolWeigh;

		public float EnemyLeftManaWeigh;

		public float EnemyAverageHpWeigh;

		public float timeMin;

		public float timeMax;

		public float leagueSurviveHeroMin;

		public float leagueSurviveHeroMax;

		public float leagueSurviveMonsterMin;

		public float leagueSurviveMonsterMax;

		public float leagueSkillCoolMin;

		public float leagueSkillCoolMax;

		public float leagueLeftManaMin;

		public float leagueLeftManaMax;

		public float leagueAverageHpMin;

		public float leagueAverageHpMax;

		public float enemySurviveHeroMax;

		public float enemySurviveHeroMin;

		public float enemySurviveMonsterMax;

		public float enemySurviveMonsterMin;

		public float enemySkillCoolMin;

		public float enemySkillCoolMax;

		public float enemyLeftManaMin;

		public float enemyLeftManaMax;

		public float enemyAverageHpMin;

		public float enemyAverageHpMax;

		public FactorWeigh()
		{
			this.timeInterval = 1f;
			this.timeWeigh = 0.1f;
			this.leagueAverageHpWeigh = 10f;
			this.leagueLeftManaWeigh = 100f;
			this.leagueSkillCoolWeigh = 1f;
			this.leagueSurviveHeroWeigh = 3f;
			this.leagueSurviveMonsterWeigh = 1f;
			this.EnemyAverageHpWeigh = 5f;
			this.EnemyLeftManaWeigh = 100f;
			this.EnemySkillCoolWeigh = -1f;
			this.EnemySurviveHeroWeigh = -3f;
			this.EnemySurviveMonsterWeigh = -1f;
			this.timeMin = -8f;
			this.timeMax = 8f;
			this.leagueAverageHpMax = 10f;
			this.leagueAverageHpMin = 0f;
			this.leagueLeftManaMax = 5f;
			this.leagueLeftManaMin = 0f;
			this.leagueSkillCoolMax = 5f;
			this.leagueSkillCoolMin = 0f;
			this.leagueSurviveHeroMax = 9f;
			this.leagueSurviveHeroMin = 0f;
			this.leagueSurviveMonsterMax = 3f;
			this.leagueSurviveMonsterMin = 0f;
			this.enemyAverageHpMax = 0f;
			this.enemyAverageHpMin = -5f;
			this.enemyLeftManaMax = 0f;
			this.enemyLeftManaMin = -5f;
			this.enemySkillCoolMax = 0f;
			this.enemySkillCoolMin = -5f;
			this.enemySurviveHeroMax = 0f;
			this.enemySurviveHeroMin = -9f;
			this.enemySurviveMonsterMax = 0f;
			this.enemySurviveMonsterMin = -3f;
		}
	}

	private enum Group
	{
		League,
		Enemy
	}

	public TeamType team;

	private float m_timeFactor;

	private float m_leagueLeftManaFactor;

	private float m_enemyLeftManaFactor;

	private float m_leagueSkillCoolFactor;

	private float m_enemySkillCoolFactor;

	private float m_leagueAverageHpFactor;

	private float m_enemyAverageHpFactor;

	private float m_leagueSurviveMonsterFactor;

	private float m_enemySurviveMonsterFactor;

	private float m_leagueSurviveHeroFactor;

	private float m_enemySurviveHeroFactor;

	private float m_strategyFactor;

	private List<int> allStrategyTypes;

	private float interval = 0.2f;

	private float lastUpdateTime;

	private StrategyHelper helper;

	private Strategy.FactorWeigh factorWeigh = new Strategy.FactorWeigh();

	public float StrategyFactor
	{
		get
		{
			this.m_strategyFactor = this.TimeFactor + this.LeagueAverageHpFactor + this.LeagueLeftManaFactor + this.LeagueSkillCoolFactor + this.LeagueSurviveHeroFactor + this.LeagueSurviveMonsterFactor + this.EnemyAverageHpFactor + this.EnemyLeftManaFactor + this.EnemySkillCoolFactor + this.EnemySurviveHeroFactor + this.EnemySurviveMonsterFactor;
			return this.m_strategyFactor;
		}
	}

	public float TimeFactor
	{
		get
		{
			if (this.m_timeFactor < this.factorWeigh.timeMin)
			{
				this.m_timeFactor = this.factorWeigh.timeMin;
			}
			else if (this.m_timeFactor > this.factorWeigh.timeMax)
			{
				this.m_timeFactor = this.factorWeigh.timeMax;
			}
			return this.m_timeFactor;
		}
	}

	public float LeagueLeftManaFactor
	{
		get
		{
			if (this.m_leagueLeftManaFactor < this.factorWeigh.leagueLeftManaMin)
			{
				this.m_leagueLeftManaFactor = this.factorWeigh.leagueLeftManaMin;
			}
			else if (this.m_leagueLeftManaFactor > this.factorWeigh.leagueLeftManaMax)
			{
				this.m_leagueLeftManaFactor = this.factorWeigh.leagueLeftManaMax;
			}
			return this.m_leagueLeftManaFactor;
		}
	}

	public float EnemyLeftManaFactor
	{
		get
		{
			if (this.m_enemyLeftManaFactor > this.factorWeigh.enemyLeftManaMax)
			{
				this.m_enemyLeftManaFactor = this.factorWeigh.enemyLeftManaMax;
			}
			else if (this.m_enemyLeftManaFactor < this.factorWeigh.enemyLeftManaMin)
			{
				this.m_enemyLeftManaFactor = this.factorWeigh.enemyLeftManaMin;
			}
			return this.m_enemyLeftManaFactor;
		}
	}

	public float LeagueSkillCoolFactor
	{
		get
		{
			if (this.m_leagueSkillCoolFactor > this.factorWeigh.leagueSkillCoolMax)
			{
				this.m_leagueSkillCoolFactor = this.factorWeigh.leagueSkillCoolMax;
			}
			return this.m_leagueSkillCoolFactor;
		}
	}

	public float EnemySkillCoolFactor
	{
		get
		{
			if (this.m_enemySkillCoolFactor < this.factorWeigh.enemySkillCoolMin)
			{
				this.m_enemySkillCoolFactor = this.factorWeigh.enemySkillCoolMin;
			}
			return this.m_enemySkillCoolFactor;
		}
	}

	public float LeagueAverageHpFactor
	{
		get
		{
			if (this.m_leagueAverageHpFactor > this.factorWeigh.leagueAverageHpMax)
			{
				this.m_leagueAverageHpFactor = this.factorWeigh.leagueAverageHpMax;
			}
			return this.m_leagueAverageHpFactor;
		}
	}

	public float EnemyAverageHpFactor
	{
		get
		{
			if (this.m_enemyAverageHpFactor < this.factorWeigh.enemyAverageHpMin)
			{
				this.m_enemyAverageHpFactor = this.factorWeigh.enemyAverageHpMin;
			}
			return this.m_enemyAverageHpFactor;
		}
	}

	public float LeagueSurviveMonsterFactor
	{
		get
		{
			if (this.m_leagueSurviveMonsterFactor > this.factorWeigh.leagueSurviveMonsterMax)
			{
				this.m_leagueSurviveMonsterFactor = this.factorWeigh.leagueSurviveMonsterMax;
			}
			return this.m_leagueSurviveMonsterFactor;
		}
	}

	public float EnemySurviveMonsterFactor
	{
		get
		{
			if (this.m_enemySurviveMonsterFactor < this.factorWeigh.enemySurviveMonsterMin)
			{
				this.m_enemySurviveMonsterFactor = this.factorWeigh.enemySurviveMonsterMin;
			}
			return this.m_enemySurviveMonsterFactor;
		}
	}

	public float LeagueSurviveHeroFactor
	{
		get
		{
			if (this.m_leagueSurviveHeroFactor > this.factorWeigh.leagueSurviveHeroMax)
			{
				this.m_leagueSurviveHeroFactor = this.factorWeigh.leagueSurviveHeroMax;
			}
			return this.m_leagueSurviveHeroFactor;
		}
	}

	public float EnemySurviveHeroFactor
	{
		get
		{
			if (this.m_enemySurviveHeroFactor < this.factorWeigh.enemySurviveHeroMin)
			{
				this.m_enemySurviveHeroFactor = this.factorWeigh.enemySurviveHeroMin;
			}
			return this.m_enemySurviveHeroFactor;
		}
	}

	public Strategy(TeamType type)
	{
		this.team = type;
		this.Init();
	}

	private void Init()
	{
		this.allStrategyTypes = new List<int>();
		for (int i = 1; i <= Enum.GetNames(Type.GetType("StrategyFactorType")).Length; i++)
		{
			this.allStrategyTypes.Add(i);
		}
		this.m_timeFactor = this.factorWeigh.timeMin;
		this.m_leagueSurviveHeroFactor = this.factorWeigh.leagueSurviveHeroMax;
		this.m_leagueAverageHpFactor = this.factorWeigh.leagueAverageHpMax;
		this.m_leagueLeftManaFactor = this.factorWeigh.leagueLeftManaMax;
		this.m_leagueSkillCoolFactor = this.factorWeigh.leagueSkillCoolMax;
		this.m_leagueSurviveMonsterFactor = this.factorWeigh.leagueSurviveMonsterMax;
		this.m_enemyAverageHpFactor = this.factorWeigh.enemyAverageHpMin;
		this.m_enemyLeftManaFactor = this.factorWeigh.enemyLeftManaMin;
		this.m_enemySkillCoolFactor = this.factorWeigh.enemySkillCoolMin;
		this.m_enemySurviveHeroFactor = this.factorWeigh.enemySurviveHeroMin;
		this.m_enemySurviveMonsterFactor = this.factorWeigh.enemySurviveMonsterMin;
		this.helper = StrategyHelper.Instance;
		this.lastUpdateTime = Time.time;
		StrategyTimer.Instance.OnInit();
	}

	public void Update()
	{
		StrategyTimer.Instance.Update();
		if (Time.time < this.lastUpdateTime + this.interval)
		{
			return;
		}
		this.lastUpdateTime = Time.time;
		foreach (int current in this.allStrategyTypes)
		{
			this.UpdateFactorByType((StrategyFactorType)current);
		}
	}

	public void OnFinish()
	{
		StrategyTimer.Instance.OnFinish();
	}

	public void UpdateFactorByType(StrategyFactorType type)
	{
		switch (type)
		{
		case StrategyFactorType.BattleTime:
			this.DoTimeFactor();
			break;
		case StrategyFactorType.LeagueLeftMana:
			this.DoLeftManaFactor(Strategy.Group.League, out this.m_leagueLeftManaFactor, this.factorWeigh.leagueLeftManaWeigh, this.factorWeigh.leagueLeftManaMin, this.factorWeigh.leagueLeftManaMax);
			break;
		case StrategyFactorType.LeagueSkillCool:
		{
			IList<Units> heros = this.helper.GetAliveLeagueHeros(this.team);
			this.DoSkillCoolFactor(heros, out this.m_leagueSkillCoolFactor, this.factorWeigh.leagueSkillCoolWeigh, this.factorWeigh.leagueSkillCoolMin, this.factorWeigh.leagueSkillCoolMax, Strategy.Group.League);
			break;
		}
		case StrategyFactorType.LeagueAverageHp:
		{
			IList<Units> heros = this.helper.GetAliveLeagueHeros(this.team);
			this.DoAverageHpFactor(heros, out this.m_leagueAverageHpFactor, this.factorWeigh.leagueAverageHpWeigh, this.factorWeigh.leagueAverageHpMin, this.factorWeigh.leagueAverageHpMax, Strategy.Group.League);
			break;
		}
		case StrategyFactorType.LeagueSurviveMonster:
		{
			int num = this.helper.GetAliveLeagueMonsterNum(this.team);
			this.DoSurviveFactor(num, out this.m_leagueSurviveMonsterFactor, this.factorWeigh.leagueSurviveMonsterWeigh, this.factorWeigh.leagueSurviveMonsterMin, this.factorWeigh.leagueSurviveMonsterMax);
			break;
		}
		case StrategyFactorType.LeagueSurviveHero:
		{
			int num = this.helper.GetAliveLeagueHeroNum(this.team);
			this.DoSurviveFactor(num, out this.m_leagueSurviveHeroFactor, this.factorWeigh.leagueSurviveHeroWeigh, this.factorWeigh.leagueSurviveHeroMin, this.factorWeigh.leagueSurviveHeroMax);
			break;
		}
		case StrategyFactorType.EnemyLeftMana:
			this.DoLeftManaFactor(Strategy.Group.Enemy, out this.m_enemyLeftManaFactor, this.factorWeigh.EnemyLeftManaWeigh, this.factorWeigh.enemyLeftManaMin, this.factorWeigh.enemyLeftManaMax);
			break;
		case StrategyFactorType.EnemySkillCool:
		{
			IList<Units> heros = this.helper.GetAliveEnemyHeros(this.team);
			this.DoSkillCoolFactor(heros, out this.m_enemySkillCoolFactor, this.factorWeigh.EnemySkillCoolWeigh, this.factorWeigh.enemySkillCoolMin, this.factorWeigh.enemySkillCoolMax, Strategy.Group.Enemy);
			break;
		}
		case StrategyFactorType.EnmeyAverageHp:
		{
			IList<Units> heros = this.helper.GetAliveEnemyHeros(this.team);
			this.DoAverageHpFactor(heros, out this.m_enemyAverageHpFactor, this.factorWeigh.EnemyAverageHpWeigh, this.factorWeigh.enemyAverageHpMin, this.factorWeigh.enemyAverageHpMax, Strategy.Group.Enemy);
			break;
		}
		case StrategyFactorType.EnemySurviveMonster:
		{
			int num = this.helper.GetAliveEnemyMonsterNum(this.team);
			this.DoSurviveFactor(num, out this.m_enemySurviveMonsterFactor, this.factorWeigh.EnemySurviveMonsterWeigh, this.factorWeigh.enemySurviveMonsterMin, this.factorWeigh.enemySurviveMonsterMax);
			break;
		}
		case StrategyFactorType.EnemySurviveHero:
		{
			int num = this.helper.GetAliveEnemyHeroNum(this.team);
			this.DoSurviveFactor(num, out this.m_enemySurviveHeroFactor, this.factorWeigh.EnemySurviveHeroWeigh, this.factorWeigh.enemySurviveHeroMin, this.factorWeigh.enemySurviveHeroMax);
			break;
		}
		}
	}

	private void DoTimeFactor()
	{
		this.m_timeFactor = this.factorWeigh.timeMin + (float)StrategyTimer.Instance.Seconds * this.factorWeigh.timeWeigh;
		if (this.m_timeFactor > this.factorWeigh.timeMax)
		{
			this.m_timeFactor = this.factorWeigh.timeMax;
		}
	}

	private void DoSurviveFactor(int num, out float factor, float weigh, float min, float max)
	{
		factor = (float)num * weigh;
		if (factor > max)
		{
			factor = max;
		}
		else if (factor < min)
		{
			factor = min;
		}
	}

	private void DoLeftManaFactor(Strategy.Group group, out float factor, float weigh, float minVal, float maxVal)
	{
		int num;
		IList<Units> list;
		if (group == Strategy.Group.League)
		{
			num = this.helper.GetAliveLeagueHeroNum(this.team);
			list = this.helper.GetAliveLeagueHeros(this.team);
		}
		else
		{
			num = this.helper.GetAliveEnemyHeroNum(this.team);
			list = this.helper.GetAliveEnemyHeros(this.team);
		}
		if (num <= 0)
		{
			if (group == Strategy.Group.League)
			{
				factor = minVal;
			}
			else
			{
				factor = maxVal;
			}
			return;
		}
		float num2 = 0f;
		foreach (Units current in list)
		{
			num2 += current.mp;
		}
		float max = num2 * (1f / weigh) * (float)(1 / num);
		float num3 = UnityEngine.Random.Range(0f, max);
		if (group == Strategy.Group.League)
		{
			factor = num3;
		}
		else
		{
			factor = -num3;
		}
		if (factor > maxVal)
		{
			factor = maxVal;
		}
		else if (factor < minVal)
		{
			factor = minVal;
		}
	}

	private void DoAverageHpFactor(IList<Units> heros, out float factor, float weigh, float minVal, float maxVal, Strategy.Group group)
	{
		if (heros != null)
		{
			float num = 0f;
			int count = heros.Count;
			foreach (Units current in heros)
			{
				num += current.hp / current.hp_max;
			}
			float max = num / (float)count * weigh;
			float num2 = UnityEngine.Random.Range(0f, max);
			if (group == Strategy.Group.League)
			{
				factor = num2;
			}
			else
			{
				factor = -num2;
			}
			if (factor > maxVal)
			{
				factor = maxVal;
			}
			else if (factor < minVal)
			{
				factor = minVal;
			}
		}
		else
		{
			factor = 0f;
		}
	}

	private void DoSkillCoolFactor(IList<Units> heros, out float factor, float weigh, float minVal, float maxVal, Strategy.Group group)
	{
		if (heros != null)
		{
			int num = 0;
			foreach (Units current in heros)
			{
				SkillManager unitComponent = current.GetUnitComponent<SkillManager>();
				if (unitComponent == null)
				{
					Debug.LogError("DoSkillCoolFactor方法，获得的skillMgr为空。");
					break;
				}
				List<string> unlockSkills = unitComponent.GetUnlockSkills();
				if (unlockSkills == null)
				{
					Debug.LogError("DoSkillCoolFactor方法，获得的skills为空。");
					break;
				}
				foreach (string current2 in unlockSkills)
				{
					Skill skillById = unitComponent.getSkillById(current2);
					if (skillById == null)
					{
						Debug.LogError("DoSkillCoolFactor方法，获得的skill为空。");
						break;
					}
					if (skillById.IsInitiativeSkill && skillById.IsCDTimeOver)
					{
						num++;
					}
				}
			}
			factor = (float)num * weigh;
			if (factor > maxVal)
			{
				factor = maxVal;
			}
			else if (factor < minVal)
			{
				factor = minVal;
			}
		}
		else if (group == Strategy.Group.League)
		{
			factor = minVal;
		}
		else
		{
			factor = maxVal;
		}
	}
}
