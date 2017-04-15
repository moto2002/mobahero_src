using Com.Game.Data;
using Com.Game.Manager;
using Common;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class LevelVo
{
	public int battle_type;

	public string battle_id;

	public string level_id;

	public int level_index;

	public bool unlock;

	private bool _isConnectPveServer;

	private ESceneBelongedBattleType _curBelongedBattleType;

	private PvpJoinType _curPvpJoinType = PvpJoinType.Invalid;

	private Dictionary<TeamType, List<EntityVo>> _teamHeroes = new Dictionary<TeamType, List<EntityVo>>();

	public void Set(int battle_type, string battle, string level, PvpJoinType inJoinType, int index)
	{
		this.battle_type = battle_type;
		this.battle_id = battle;
		this.level_id = level;
		this.level_index = index;
		if (StringUtils.CheckValid(level))
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(level);
			if (dataById != null)
			{
				this._isConnectPveServer = (dataById.is_connect_pveserver > 0);
				this._curBelongedBattleType = (ESceneBelongedBattleType)dataById.belonged_battletype;
			}
		}
		this._curPvpJoinType = inJoinType;
	}

	public void SetTeamHeroes(TeamType team, List<EntityVo> heros)
	{
		this._teamHeroes[team] = heros;
	}

	public void SetAllHeroes(IDictionary<TeamType, List<EntityVo>> list)
	{
		this._teamHeroes.Clear();
		foreach (TeamType current in list.Keys)
		{
			this._teamHeroes[current] = list[current];
		}
	}

	public List<EntityVo> GetTeamHeroes(TeamType team)
	{
		List<EntityVo> result;
		this._teamHeroes.TryGetValue(team, out result);
		return result;
	}

	public bool IsConnectPveServer()
	{
		return this._isConnectPveServer;
	}

	public bool IsDaLuanDouPvp()
	{
		return this._curBelongedBattleType == ESceneBelongedBattleType.DaLuanDouPvp;
	}

	public bool IsRank()
	{
		return this._curBelongedBattleType == ESceneBelongedBattleType.Rank;
	}

	public bool IsRank(string battleId)
	{
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
		return dataById != null && dataById.belonged_battletype == 4;
	}

	public bool IsSolo(string battleId)
	{
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
		return dataById != null && dataById.hero1_number_cap == 1;
	}

	private bool IsSingleOrTeamMatch()
	{
		return this._curPvpJoinType == PvpJoinType.Single || this._curPvpJoinType == PvpJoinType.Team;
	}

	public bool IsRandomSelectHero()
	{
		return this.IsDaLuanDouPvp() && this.IsSingleOrTeamMatch();
	}

	public bool IsFightWithRobot()
	{
		return this._curBelongedBattleType == ESceneBelongedBattleType.FightWithRobotEasy || this._curBelongedBattleType == ESceneBelongedBattleType.FightWithRobotNormal || this._curBelongedBattleType == ESceneBelongedBattleType.FightWithRobotHard;
	}

	public bool IsFightWithRobot(string battleId)
	{
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
		if (dataById != null)
		{
			ESceneBelongedBattleType belonged_battletype = (ESceneBelongedBattleType)dataById.belonged_battletype;
			if (belonged_battletype == ESceneBelongedBattleType.FightWithRobotEasy || belonged_battletype == ESceneBelongedBattleType.FightWithRobotNormal || belonged_battletype == ESceneBelongedBattleType.FightWithRobotHard)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsBattleNewbieGuide()
	{
		return this._curBelongedBattleType == ESceneBelongedBattleType.NewbieElementaryOneOne || this._curBelongedBattleType == ESceneBelongedBattleType.NewbieElementaryFiveFive;
	}

	public bool Is3V3V3()
	{
		return this._curBelongedBattleType == ESceneBelongedBattleType.Pvp3v3v3;
	}

	public bool IsLeague(string battleId)
	{
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
		return dataById != null && dataById.task_battletype == "4";
	}
}
