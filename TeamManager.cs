using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : Singleton<TeamManager>
{
	private const string TEAM_1 = "LM_TEAM";

	private const string TEAM_2 = "BL_TEAM";

	private const string TEAM_3 = "NE_TEAM";

	private const string TEAM_4 = "3_TEAM";

	public static Team[] teams;

	public static TeamType MyTeam
	{
		get
		{
			if (LevelManager.CurBattleType == 12)
			{
				return Singleton<PvpManager>.Instance.SelfTeamType;
			}
			return TeamType.LM;
		}
	}

	public static IList<TeamType> EnemyTeams
	{
		get
		{
			return TeamManager.GetEnemyTeams(TeamManager.MyTeam);
		}
	}

	static TeamManager()
	{
		TeamManager.Init();
	}

	private static void Init()
	{
		TeamManager.teams = new Team[4];
		TeamManager.teams[0] = new Team(0, "LM_TEAM");
		TeamManager.teams[1] = new Team(1, "BL_TEAM");
		TeamManager.teams[2] = new Team(2, "NE_TEAM");
		TeamManager.teams[3] = new Team(3, "3_TEAM");
		TeamManager.teams[0].setRelation(TeamManager.teams, new Relation[]
		{
			Relation.Companion,
			Relation.Hostility,
			Relation.Neutral,
			Relation.Hostility
		});
		TeamManager.teams[1].setRelation(TeamManager.teams, new Relation[]
		{
			Relation.Hostility,
			Relation.Companion,
			Relation.Neutral,
			Relation.Hostility
		});
		TeamManager.teams[2].setRelation(TeamManager.teams, new Relation[]
		{
			Relation.Hostility,
			Relation.Hostility,
			Relation.Companion,
			Relation.Hostility
		});
		TeamManager.teams[3].setRelation(TeamManager.teams, new Relation[]
		{
			Relation.Hostility,
			Relation.Hostility,
			Relation.Neutral,
			Relation.Companion
		});
	}

	public static Relation GetRelation(Units a, Units b)
	{
		Relation result = Relation.Neutral;
		if (a != null && b != null)
		{
			result = a.team.getRelation(b.team);
		}
		return result;
	}

	public static bool CanAttack(Units theurgist, Units target)
	{
		if (null == theurgist)
		{
			return false;
		}
		if (null == target)
		{
			return false;
		}
		if (target.isBuffItem || target.isItem)
		{
			return false;
		}
		Relation relation = TeamManager.GetRelation(theurgist, target);
		return relation == Relation.Hostility || relation == Relation.Neutral;
	}

	public static bool CanFollow(Units theurgist, Units target)
	{
		Relation relation = TeamManager.GetRelation(theurgist, target);
		return relation == Relation.Companion;
	}

	public static bool CanAssist(Units theurgist, Units target)
	{
		Relation relation = TeamManager.GetRelation(theurgist, target);
		return relation == Relation.Friendly || relation == Relation.Companion;
	}

	public static List<int> GetOtherTeam(Team team, Relation relation, bool isContainNeutral = true)
	{
		if (TeamManager.teams != null)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < TeamManager.teams.Length; i++)
			{
				Team team2 = TeamManager.teams[i];
				if (relation == Relation.All)
				{
					list.Add(team2.ID);
				}
				else if (team.getRelation(team2) == relation || (team.getRelation(team2) == Relation.Neutral && isContainNeutral))
				{
					list.Add(team2.ID);
				}
			}
			return list;
		}
		return null;
	}

	public static string GetTeamName(TeamType teamType)
	{
		switch (teamType)
		{
		case TeamType.LM:
			return "LM";
		case TeamType.BL:
			return "BL";
		case TeamType.Neutral:
			return "NE";
		case TeamType.Team_3:
			return "Team_3";
		default:
			return "Neutral";
		}
	}

	public static bool CheckTeam(GameObject target, List<int> teamId)
	{
		for (int i = 0; i < teamId.Count; i++)
		{
			Team team = TeamManager.teams[teamId[i]];
			string name = team.name;
			if (target.name.Contains(name))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckTeam(GameObject target, int teamId)
	{
		Team team = TeamManager.teams[teamId];
		string name = team.name;
		return target.name.Contains(name);
	}

	public static bool CheckRelation(int s_team_id, int t_team_id, Relation re)
	{
		Team team = TeamManager.teams[s_team_id];
		Team otherTeam = TeamManager.teams[t_team_id];
		Relation relation = team.getRelation(otherTeam);
		return relation == re;
	}

	public static bool CheckTeamType(int s_team_id, int t_team_id)
	{
		return t_team_id == s_team_id;
	}

	public static bool CheckTeamType(int source, List<int> targets)
	{
		return targets != null && targets.Contains(source);
	}

	public static bool CheckTeam(GameObject self, GameObject target, SkillTargetCamp targetCampType, Units parentUnit = null)
	{
		switch (targetCampType)
		{
		case SkillTargetCamp.Self:
			return self && target && self.name.Equals(target.name);
		case SkillTargetCamp.Enemy:
		case SkillTargetCamp.AttackYouTarget:
		case SkillTargetCamp.AttackTarget:
			return TeamManager.CheckTeam(target, TeamManager.GetOtherTeam(TeamManager.GetTeam(self), Relation.Hostility, true));
		case SkillTargetCamp.Partener:
			return TeamManager.CheckTeam(target, TeamManager.GetOtherTeam(TeamManager.GetTeam(self), Relation.Companion, false));
		case SkillTargetCamp.All:
			return true;
		case SkillTargetCamp.AllWhitOutSelf:
			return !self.name.Equals(target.name);
		case SkillTargetCamp.ParentObj:
			return parentUnit != null && target != null && parentUnit.name.Equals(target.name);
		case SkillTargetCamp.AllWhitOutPartener:
			return (self && target && self.name.Equals(target.name)) || !TeamManager.CheckTeam(target, TeamManager.GetOtherTeam(TeamManager.GetTeam(self), Relation.Companion, false));
		}
		return false;
	}

	public static Team GetTeam(GameObject gameObj)
	{
		if (gameObj != null)
		{
			if (gameObj.name.Contains("LM_TEAM"))
			{
				return TeamManager.teams[0];
			}
			if (gameObj.name.Contains("BL_TEAM"))
			{
				return TeamManager.teams[1];
			}
			if (gameObj.name.Contains("NE_TEAM"))
			{
				return TeamManager.teams[2];
			}
			if (gameObj.name.Contains("3_TEAM"))
			{
				return TeamManager.teams[3];
			}
		}
		return null;
	}

	public static IList<TeamType> GetEnemyTeams(Units u)
	{
		return TeamManager.GetEnemyTeams(u.TeamType);
	}

	public static IList<TeamType> GetEnemyTeams(TeamType team)
	{
		List<TeamType> list = new List<TeamType>();
		for (int i = 0; i < 4; i++)
		{
			if (team != (TeamType)i)
			{
				list.Add((TeamType)i);
			}
		}
		return list;
	}

	public static TeamType AdjustTeam(TeamType team)
	{
		if ((team == TeamType.LM || team == TeamType.BL) && TeamManager.MyTeam == TeamType.BL)
		{
			return (team != TeamType.BL) ? TeamType.BL : TeamType.LM;
		}
		return team;
	}

	public static bool IsEnemy(Units unit)
	{
		return unit.TeamType != TeamManager.MyTeam;
	}

	public static bool IsEnemy(TeamType team)
	{
		return team != TeamManager.MyTeam;
	}
}
