using System;
using System.Collections.Generic;

internal class StrategyHelper
{
	private static StrategyHelper _instance;

	private Dictionary<TeamType, Dictionary<TargetTag, Units>> cacheUnits;

	public static StrategyHelper Instance
	{
		get
		{
			if (StrategyHelper._instance == null)
			{
				StrategyHelper._instance = new StrategyHelper();
			}
			return StrategyHelper._instance;
		}
	}

	private StrategyHelper()
	{
		this.cacheUnits = new Dictionary<TeamType, Dictionary<TargetTag, Units>>();
	}

	private IList<Units> GetAliveByType(TeamType teamType, TargetTag tag)
	{
		return MapManager.Instance.GetMapUnits(teamType, tag);
	}

	public IList<Units> GetAliveLeagueHeros(TeamType self)
	{
		return this.GetAliveByType(self, TargetTag.Hero);
	}

	public IList<Units> GetAliveLeagueMonsters(TeamType self)
	{
		return this.GetAliveByType(self, TargetTag.Monster);
	}

	public int GetAliveLeagueMonsterNum(TeamType self)
	{
		IList<Units> aliveLeagueMonsters = this.GetAliveLeagueMonsters(self);
		if (aliveLeagueMonsters == null)
		{
			return 0;
		}
		return aliveLeagueMonsters.Count;
	}

	public int GetAliveLeagueHeroNum(TeamType self)
	{
		IList<Units> aliveLeagueHeros = this.GetAliveLeagueHeros(self);
		if (aliveLeagueHeros == null)
		{
			return 0;
		}
		return aliveLeagueHeros.Count;
	}

	public int GetAliveEnemyMonsterNum(TeamType self)
	{
		IList<Units> aliveEnemyMonsters = this.GetAliveEnemyMonsters(self);
		if (aliveEnemyMonsters == null)
		{
			return 0;
		}
		return aliveEnemyMonsters.Count;
	}

	public int GetAliveEnemyHeroNum(TeamType self)
	{
		IList<Units> aliveEnemyHeros = this.GetAliveEnemyHeros(self);
		if (aliveEnemyHeros == null)
		{
			return 0;
		}
		return aliveEnemyHeros.Count;
	}

	public IList<Units> GetAliveEnemyMonsters(TeamType self)
	{
		return this.GetAliveByType((self != TeamType.LM) ? TeamType.LM : TeamType.BL, TargetTag.Monster);
	}

	public IList<Units> GetAliveEnemyHeros(TeamType self)
	{
		return this.GetAliveByType((self != TeamType.LM) ? TeamType.LM : TeamType.BL, TargetTag.Hero);
	}
}
