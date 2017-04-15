using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AchieveData
{
	private const float Interval = 7f;

	private readonly int _heroId;

	private readonly List<float> _killTime = new List<float>();

	private readonly List<EAchievementType> _allAchievemets = new List<EAchievementType>();

	private readonly string _heroName;

	private readonly TeamType _heroTeam;

	public string unittype;

	public string HeroName
	{
		get
		{
			return this._heroName;
		}
	}

	public int HeroId
	{
		get
		{
			return this._heroId;
		}
	}

	public List<float> KillTime
	{
		get
		{
			return this._killTime;
		}
	}

	public int TotalKill
	{
		get;
		private set;
	}

	public int ContinusKillNoTime
	{
		get;
		set;
	}

	public string SummerName
	{
		get;
		set;
	}

	public int ContinusKillWithTime
	{
		get;
		set;
	}

	public List<EAchievementType> AllAchievements
	{
		get
		{
			return this._allAchievemets;
		}
	}

	public int SelfDeathTime
	{
		get;
		private set;
	}

	public int MonsterKillNum
	{
		get;
		private set;
	}

	public int TowerDestroyNum
	{
		get;
		protected set;
	}

	public static int TowerDestrouByLm
	{
		get;
		protected set;
	}

	public static int TowerDestroyByBl
	{
		get;
		protected set;
	}

	public static int EpicMonsterLm
	{
		get;
		protected set;
	}

	public static int EpicMonsterBl
	{
		get;
		protected set;
	}

	public AchieveData(int id, string heroName, TeamType team)
	{
		this._heroId = id;
		this.TotalKill = 0;
		this.ContinusKillNoTime = 0;
		this.ContinusKillWithTime = 0;
		this._heroName = heroName;
		this._heroTeam = team;
		this.SelfDeathTime = 0;
		this.SummerName = string.Empty;
		this.unittype = string.Empty;
	}

	public void UpdateKillingHeroData(AchieveData targetData)
	{
		if (targetData.HeroId != this._heroId)
		{
			this.ContinusKillNoTime++;
			this.TotalKill++;
			if (this._killTime.Count > 0)
			{
				float num = this._killTime[this._killTime.Count - 1];
				if (Time.time - num <= 7f)
				{
					this.ContinusKillWithTime++;
				}
				else
				{
					this.ContinusKillWithTime = 1;
				}
			}
			else
			{
				this.ContinusKillWithTime = 1;
			}
			this._killTime.Add(Time.time);
		}
		else
		{
			this.ContinusKillNoTime = 0;
			this.ContinusKillWithTime = 0;
			this.SelfDeathTime++;
		}
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.UpdateView);
	}

	public void UpdateKillingMonsterData()
	{
		this.MonsterKillNum++;
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.UpdateView);
	}

	public static void updataMusicState(TeamType team)
	{
		bool flag = false;
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
		string[] array = new string[]
		{
			"Play_Amb_1v1_map16",
			"Play_Amb_2v2_map11",
			"Play_Amb_2v2_map13",
			"Play_Amb_2v2_map14",
			"Play_Amb_5v5_map17"
		};
		string text = dataById.scene_map_id;
		text = text.ToLower();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Contains(text))
			{
				if (array[i].Contains("1v1"))
				{
					text = "1v1";
				}
				else if (array[i].Contains("5v5"))
				{
					text = "3v3";
				}
				break;
			}
		}
		if (text.Contains("3v3"))
		{
			flag = true;
		}
		if (flag)
		{
			if (TeamManager.MyTeam == team)
			{
				if (team == TeamType.LM && AchieveData.TowerDestrouByLm == 1)
				{
					BgmPlayer.OnFirstTowerBePullbyUS();
				}
				else if (team == TeamType.BL && AchieveData.TowerDestroyByBl == 1)
				{
					BgmPlayer.OnFirstTowerBePullbyUS();
				}
			}
			if (AchieveData.TowerDestroyByBl == 2 || AchieveData.TowerDestrouByLm == 2)
			{
				BgmPlayer.OnFstOr2ndTowerBePullDown();
			}
		}
		else if (AchieveData.TowerDestrouByLm == 1 || AchieveData.TowerDestroyByBl == 1)
		{
			BgmPlayer.OnFstOr2ndTowerBePullDown();
		}
	}

	public static void UpdateTowerDestroyByTeam(TeamType team)
	{
		if (team == TeamType.LM)
		{
			AchieveData.TowerDestrouByLm++;
		}
		else if (team == TeamType.BL)
		{
			AchieveData.TowerDestroyByBl++;
		}
		AchieveData.updataMusicState(team);
	}

	public static void UpdateEpicMonster(TeamType team)
	{
		if (team == TeamType.LM)
		{
			AchieveData.EpicMonsterLm++;
		}
		else if (team == TeamType.BL)
		{
			AchieveData.EpicMonsterBl++;
		}
	}

	public static void Clear()
	{
		AchieveData.TowerDestrouByLm = 0;
		AchieveData.TowerDestroyByBl = 0;
		AchieveData.EpicMonsterBl = 0;
		AchieveData.EpicMonsterLm = 0;
	}

	public void UpdateTowerDestroy()
	{
		this.TowerDestroyNum++;
	}

	public void CheckAchievemtCondition(AchieveData attackData, AchieveData targetData, KillType killtype = KillType.Normal)
	{
		EAchievementType conditionResult = this.GetConditionResult(attackData, targetData, killtype);
		string promptId = this.GetPromptId(attackData, targetData, killtype, conditionResult);
		if (conditionResult != EAchievementType.None)
		{
			if (LevelManager.Instance.IsPvpBattleType)
			{
				AchieveHelper.BrocastMsg(promptId, this.HeroName, targetData.HeroName, attackData._heroTeam, targetData._heroTeam, this.SummerName, targetData.SummerName);
			}
			else
			{
				AchieveHelper.BrocastMsg(promptId, this.HeroName, targetData.HeroName, attackData._heroTeam, targetData._heroTeam, string.Empty, string.Empty);
			}
		}
		else
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Achievement is none, 连杀:",
				this.ContinusKillWithTime,
				" 累积杀:",
				this.ContinusKillNoTime
			}));
		}
		if ((targetData._heroTeam == TeamType.LM && MapManager.Instance.IsHeroAllDead(TeamType.LM) && GameManager.Instance.Spawner.GetPlayerNum(TeamType.LM) >= AchieveManager.TuanmieTotalCount) || (targetData._heroTeam == TeamType.BL && MapManager.Instance.IsHeroAllDead(TeamType.BL) && GameManager.Instance.Spawner.GetPlayerNum(TeamType.BL) >= AchieveManager.TuanmieTotalCount))
		{
			promptId = this.GetPromptId(attackData, targetData, killtype, EAchievementType.TuanMie);
			if (LevelManager.Instance.IsPvpBattleType)
			{
				AchieveHelper.BrocastMsg(promptId, this.HeroName, targetData.HeroName, attackData._heroTeam, targetData._heroTeam, this.SummerName, targetData.SummerName);
			}
			else
			{
				AchieveHelper.BrocastMsg(promptId, this.HeroName, targetData.HeroName, attackData._heroTeam, targetData._heroTeam, string.Empty, string.Empty);
			}
		}
	}

	private EAchievementType GetConditionResult(AchieveData attackData, AchieveData targetData, KillType killtype = KillType.Normal)
	{
		if (LevelManager.CurBattleType == 6)
		{
			return EAchievementType.NormalKill;
		}
		EAchievementType result = EAchievementType.None;
		if (killtype == KillType.StopKill)
		{
			return EAchievementType.ZhongJie;
		}
		if (killtype == KillType.FirstBoold)
		{
			return EAchievementType.FirstBlood;
		}
		if (!LevelManager.Instance.IsPvpBattleType && GameManager.Instance.AchieveManager.AllHeroDeathNum == 1 && this.ContinusKillNoTime == 1)
		{
			result = EAchievementType.FirstBlood;
		}
		else if (attackData.ContinusKillWithTime >= AchieveManager.DoubleKillCount && attackData.ContinusKillWithTime <= AchieveManager.HexaKillCount)
		{
			if (attackData.ContinusKillWithTime == AchieveManager.DoubleKillCount)
			{
				result = EAchievementType.DoubleKill;
			}
			else if (attackData.ContinusKillWithTime == AchieveManager.TripleKillCount)
			{
				result = EAchievementType.TribleKill;
			}
			else if (attackData.ContinusKillWithTime == AchieveManager.FourthKillCount)
			{
				result = EAchievementType.FourKill;
			}
			else if (attackData.ContinusKillWithTime == AchieveManager.FifthKillCount)
			{
				result = EAchievementType.FiveKill;
			}
			else if (attackData.ContinusKillWithTime == AchieveManager.HexaKillCount)
			{
				result = EAchievementType.HexaKill;
			}
		}
		else if (targetData.ContinusKillNoTime >= AchieveManager.ZhongjieCount)
		{
			result = EAchievementType.ZhongJie;
		}
		else if (attackData.ContinusKillNoTime >= AchieveManager.DashaKillCount)
		{
			if (attackData.ContinusKillNoTime < AchieveManager.BaozouKillCount)
			{
				result = EAchievementType.DaShaTeSha;
			}
			else if (attackData.ContinusKillNoTime < AchieveManager.WurenKillCount)
			{
				result = EAchievementType.BaoZou;
			}
			else if (attackData.ContinusKillNoTime < AchieveManager.ZhuzaiKillCount)
			{
				result = EAchievementType.WuRenNengDang;
			}
			else if (attackData.ContinusKillNoTime < AchieveManager.GodlikeKillCount)
			{
				result = EAchievementType.ZhuZaiBiSai;
			}
			else if (attackData.ContinusKillNoTime < AchieveManager.LegendaryKillCount)
			{
				result = EAchievementType.GodLike;
			}
			else
			{
				result = EAchievementType.Legendary;
			}
		}
		else if (attackData.ContinusKillNoTime < AchieveManager.DashaKillCount)
		{
			result = EAchievementType.NormalKill;
		}
		targetData.UpdateKillingHeroData(targetData);
		return result;
	}

	private string GetPromptId(AchieveData attackData, AchieveData targetData, KillType killtype = KillType.Normal, EAchievementType arcType = EAchievementType.None)
	{
		Units player = PlayerControlMgr.Instance.GetPlayer();
		if (null == player)
		{
			return string.Empty;
		}
		bool flag = targetData._heroTeam != (TeamType)player.teamType;
		string result = string.Empty;
		if (arcType == EAchievementType.TuanMie)
		{
			result = ((!flag) ? "1100" : "1099");
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1103" : "1102") : "1101");
			}
			return result;
		}
		if (arcType == EAchievementType.KilledByMonster)
		{
			result = ((!flag) ? "1089" : "1088");
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1092" : "1091") : "1090");
			}
			return result;
		}
		if (killtype == KillType.StopKill)
		{
			result = ((!flag) ? "1094" : "1093");
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1097" : "1096") : "1095");
			}
			return result;
		}
		if (killtype == KillType.FirstBoold)
		{
			result = ((!flag) ? "1017" : "1016");
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1020" : "1019") : "1018");
			}
			return result;
		}
		if (!LevelManager.Instance.IsPvpBattleType && GameManager.Instance.AchieveManager.AllHeroDeathNum == 1 && this.ContinusKillNoTime == 1)
		{
			result = ((!flag) ? "1017" : "1016");
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1020" : "1019") : "1018");
			}
		}
		else if (attackData.ContinusKillWithTime >= AchieveManager.DoubleKillCount && attackData.ContinusKillWithTime <= AchieveManager.HexaKillCount)
		{
			if (attackData.ContinusKillWithTime == AchieveManager.DoubleKillCount)
			{
				result = ((!flag) ? "1026" : "1021");
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1041" : "1036") : "1031");
				}
			}
			else if (attackData.ContinusKillWithTime == AchieveManager.TripleKillCount)
			{
				result = ((!flag) ? "1027" : "1022");
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1042" : "1037") : "1032");
				}
			}
			else if (attackData.ContinusKillWithTime == AchieveManager.FourthKillCount)
			{
				result = ((!flag) ? "1028" : "1023");
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1043" : "1038") : "1033");
				}
			}
			else if (attackData.ContinusKillWithTime == AchieveManager.FifthKillCount)
			{
				result = ((!flag) ? "1029" : "1024");
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1044" : "1039") : "1034");
				}
			}
			else if (attackData.ContinusKillWithTime == AchieveManager.HexaKillCount)
			{
				result = ((!flag) ? "1030" : "1025");
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1045" : "1040") : "1035");
				}
			}
		}
		else if (targetData.ContinusKillNoTime >= AchieveManager.ZhongjieCount)
		{
			result = ((!flag) ? "1094" : "1093");
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1097" : "1096") : "1095");
			}
		}
		else if (attackData.ContinusKillNoTime >= AchieveManager.DashaKillCount)
		{
			if (attackData.ContinusKillNoTime < AchieveManager.BaozouKillCount)
			{
				result = ((!flag) ? "1052" : ((!attackData.HeroName.Equals(player.npc_id)) ? "1046" : "1058"));
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1076" : "1070") : "1064");
				}
			}
			else if (attackData.ContinusKillNoTime < AchieveManager.WurenKillCount)
			{
				result = ((!flag) ? "1053" : ((!attackData.HeroName.Equals(player.npc_id)) ? "1047" : "1059"));
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1077" : "1071") : "1065");
				}
			}
			else if (attackData.ContinusKillNoTime < AchieveManager.ZhuzaiKillCount)
			{
				result = ((!flag) ? "1054" : ((!attackData.HeroName.Equals(player.npc_id)) ? "1048" : "1060"));
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1078" : "1072") : "1066");
				}
			}
			else if (attackData.ContinusKillNoTime < AchieveManager.GodlikeKillCount)
			{
				result = ((!flag) ? "1055" : ((!attackData.HeroName.Equals(player.npc_id)) ? "1049" : "1061"));
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1079" : "1073") : "1067");
				}
			}
			else if (attackData.ContinusKillNoTime < AchieveManager.LegendaryKillCount)
			{
				result = ((!flag) ? "1056" : ((!attackData.HeroName.Equals(player.npc_id)) ? "1050" : "1062"));
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1080" : "1074") : "1068");
				}
			}
			else
			{
				result = ((!flag) ? "1057" : ((!attackData.HeroName.Equals(player.npc_id)) ? "1051" : "1063"));
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1081" : "1075") : "1069");
				}
			}
		}
		else if (attackData.ContinusKillNoTime < AchieveManager.DashaKillCount)
		{
			result = ((!flag) ? ((!targetData.HeroName.Equals(player.npc_id)) ? "1083" : "1098") : ((!attackData.HeroName.Equals(player.npc_id)) ? "1084" : "1082"));
			if (targetData.unittype.Equals("Building") || targetData.unittype.Equals("Monster"))
			{
				result = ((!flag) ? "1089" : "1088");
			}
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				result = ((targetData._heroTeam != TeamType.BL) ? ((targetData._heroTeam != TeamType.LM) ? "1087" : "1086") : "1085");
			}
		}
		targetData.UpdateKillingHeroData(targetData);
		return result;
	}
}
