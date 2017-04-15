using Com.Game.Module;
using System;
using System.Collections.Generic;

public class StatisticsManager : StaticUnitComponent
{
	private int killNum;

	private int monster_killNum;

	private int tower_killNum;

	private int hero_killNum;

	private int deathNum;

	private float getExp;

	private float getMoney;

	public static int userHeroDeadCount;

	private static Dictionary<TeamType, int> _deadCounts = new Dictionary<TeamType, int>();

	public static bool userHeroFirstBlood;

	public static int FirstBloodUnitUniqueId = 0;

	public static bool canSetHeroFirstBlood = true;

	private static bool _isGotFirstBloodGold = false;

	public static Units S_LastedDeadUnits;

	public Callback killHeroCallBack;

	public Callback DeathCallBack;

	public override void OnInit()
	{
		GameManager.Instance.DamageStatisticalManager.AddHero(this.self.unique_id, this.self);
		this.killNum = 0;
		this.monster_killNum = 0;
		this.tower_killNum = 0;
		this.hero_killNum = 0;
		this.deathNum = 0;
		this.getExp = 0f;
		this.getMoney = 0f;
	}

	public override void OnStart()
	{
	}

	public override void OnUpdate(float deltaTime)
	{
	}

	public override void OnStop()
	{
	}

	public override void OnExit()
	{
	}

	public override void OnWound(Units attacker, float value)
	{
		if (attacker != null && attacker.isHero)
		{
			GameManager.Instance.DamageStatisticalManager.AddDamage(attacker.unique_id, (int)value);
		}
	}

	public override void OnDeath(Units attacker)
	{
		if (this.self.MirrorState)
		{
			return;
		}
		StatisticsManager.S_OnDeath(this.self);
		if (attacker != null)
		{
			GameManager.Instance.AchieveManager.UpdateKillHeroData(attacker, this.self);
			GameManager.Instance.AchieveManager.UpdateMonsterKillData(attacker);
			this.killHandle(attacker);
			this.KillStatistics(attacker);
			attacker.SetLastKilledTarget(this.self);
		}
	}

	public int AddExp(int exp)
	{
		this.getExp += (float)exp;
		return (int)this.getExp;
	}

	public int AddMoney(int moneyNum)
	{
		this.getMoney += (float)moneyNum;
		return (int)this.getMoney;
	}

	public int AddKillNum()
	{
		this.killNum++;
		return this.killNum;
	}

	public int AddHeroKill()
	{
		this.hero_killNum++;
		if (this.killHeroCallBack != null)
		{
			this.killHeroCallBack();
		}
		return this.hero_killNum;
	}

	public int AddHeroDeath()
	{
		this.deathNum++;
		if (this.DeathCallBack != null)
		{
			this.DeathCallBack();
		}
		return this.deathNum;
	}

	public int AddMonsterKill()
	{
		this.monster_killNum++;
		return this.monster_killNum;
	}

	public int AddTowerKill()
	{
		this.tower_killNum++;
		return this.tower_killNum;
	}

	public int GetHeroKill()
	{
		return this.hero_killNum;
	}

	public void AddHeroKillNum()
	{
		this.hero_killNum++;
	}

	public int GetDeathNum()
	{
		return this.deathNum;
	}

	public int GetMonsterKill()
	{
		return this.monster_killNum;
	}

	public int GetTowerKill()
	{
		return this.tower_killNum;
	}

	public int GetExp()
	{
		return (int)this.getExp;
	}

	public int GetMoney()
	{
		return (int)this.getMoney;
	}

	public int GetKillNum()
	{
		return this.killNum;
	}

	public void killHandle(Units attacker)
	{
		if (attacker != null && attacker != null)
		{
			attacker.GetExp(attacker);
			attacker.killNum++;
			if (LevelManager.Instance.IsZyBattleType || LevelManager.CurBattleType == 2)
			{
				Singleton<RewardDrop>.Instance.DropItem(attacker, this.self);
			}
		}
	}

	public void KillStatistics(Units attacker)
	{
		MyStatistic.Instance.UpdateData(this.self, attacker);
		BattleAttrManager.Instance.Update();
		if (attacker == null)
		{
			return;
		}
		StatisticsManager unitComponent = attacker.GetUnitComponent<StatisticsManager>();
		if (unitComponent != null)
		{
			if (this.self.isHero)
			{
				if (attacker.tag == "Player" && !StatisticsManager.userHeroFirstBlood && StatisticsManager.canSetHeroFirstBlood)
				{
					StatisticsManager.userHeroFirstBlood = true;
					StatisticsManager.FirstBloodUnitUniqueId = attacker.unique_id;
					StatisticsManager.canSetHeroFirstBlood = false;
				}
				else if (attacker.tag == "Hero" && !StatisticsManager.userHeroFirstBlood && StatisticsManager.canSetHeroFirstBlood)
				{
					StatisticsManager.canSetHeroFirstBlood = false;
					StatisticsManager.FirstBloodUnitUniqueId = attacker.unique_id;
				}
				unitComponent.AddHeroKill();
				this.self.GetUnitComponent<StatisticsManager>().AddHeroDeath();
			}
			else if (this.self.isMonster)
			{
				unitComponent.AddMonsterKill();
			}
			else if (this.self.isTower)
			{
				unitComponent.AddTowerKill();
			}
		}
	}

	public static void S_OnInit()
	{
		StatisticsManager.ClearDeadCount();
		StatisticsManager.userHeroDeadCount = 0;
		StatisticsManager.S_LastedDeadUnits = null;
	}

	public static void S_OnDeath(Units deathUnit)
	{
		if (deathUnit.isHero)
		{
			Dictionary<TeamType, int> deadCounts;
			Dictionary<TeamType, int> expr_10 = deadCounts = StatisticsManager._deadCounts;
			TeamType teamType;
			TeamType expr_18 = teamType = deathUnit.TeamType;
			int num = deadCounts[teamType];
			expr_10[expr_18] = num + 1;
		}
		StatisticsManager.S_LastedDeadUnits = deathUnit;
		if (PlayerControlMgr.Instance.GetPlayer() == deathUnit)
		{
			StatisticsManager.userHeroDeadCount++;
		}
	}

	public static void ClearDeadCount()
	{
		for (int i = 0; i < 4; i++)
		{
			StatisticsManager._deadCounts[(TeamType)i] = 0;
		}
	}

	public static int GetDeadCount(TeamType team)
	{
		return StatisticsManager._deadCounts[team];
	}

	public static bool IsCanGetFirstBloodGold(int inAttackerId)
	{
		return !StatisticsManager._isGotFirstBloodGold && inAttackerId == StatisticsManager.FirstBloodUnitUniqueId;
	}

	public static void OperationAfterGotFirstBloodGold()
	{
		StatisticsManager._isGotFirstBloodGold = true;
	}
}
