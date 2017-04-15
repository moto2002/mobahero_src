using Assets.Scripts.Server;
using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : IGlobalComServer
{
	[SerializeField]
	public static LevelVo m_CurLevel = new LevelVo();

	private Dictionary<int, List<string>> battleList = new Dictionary<int, List<string>>();

	private Dictionary<string, List<string>> sceneList = new Dictionary<string, List<string>>();

	public Dictionary<int, List<string>> unlockBattleList = new Dictionary<int, List<string>>();

	public List<string> unlockSceneList = new List<string>();

	private bool isInitLevel;

	private static LevelManager m_Instance;

	public static int CurBattleType
	{
		get
		{
			return LevelManager.m_CurLevel.battle_type;
		}
	}

	public static string CurBattleId
	{
		get
		{
			return LevelManager.m_CurLevel.battle_id;
		}
	}

	public static bool IsTestingLevel
	{
		get
		{
			return LevelManager.CurLevelId.Equals("10061") || LevelManager.CurLevelId.Equals("10062") || LevelManager.CurLevelId.Equals("10063") || LevelManager.CurLevelId.Equals("10064") || LevelManager.CurBattleType == 6 || LevelManager.CurLevelId.Equals("10065") || LevelManager.CurLevelId.Equals("10066");
		}
	}

	public static string CurLevelId
	{
		get
		{
			return LevelManager.m_CurLevel.level_id;
		}
	}

	public static int CurLevelIndex
	{
		get
		{
			return LevelManager.m_CurLevel.level_index;
		}
	}

	public static LevelManager Instance
	{
		get
		{
			return LevelManager.m_Instance;
		}
	}

	public bool IsPvpBattleType
	{
		get
		{
			return LevelManager.CurBattleType == 12;
		}
	}

	public bool IsZyBattleType
	{
		get
		{
			return LevelManager.CurBattleType == 1;
		}
	}

	public bool IsServerZyBattleType
	{
		get
		{
			return this.IsZyBattleType && LevelManager.IsCurLevelConnectPveServer();
		}
	}

	public bool IsClientZyBattleType
	{
		get
		{
			return this.IsZyBattleType && !LevelManager.IsCurLevelConnectPveServer();
		}
	}

	public static string GetCurLevelMapName()
	{
		string curLevelId = LevelManager.CurLevelId;
		if (!StringUtils.CheckValid(curLevelId))
		{
			return string.Empty;
		}
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(curLevelId);
		if (dataById == null)
		{
			return string.Empty;
		}
		if (StringUtils.CheckValid(dataById.scene_map_id))
		{
			return dataById.scene_map_id;
		}
		return string.Empty;
	}

	private static bool IsCurLevelConnectPveServer()
	{
		return LevelManager.m_CurLevel.IsConnectPveServer();
	}

	public void OnAwake()
	{
		LevelManager.m_Instance = this;
	}

	public void OnStart()
	{
	}

	public void OnUpdate()
	{
	}

	public void OnDestroy()
	{
	}

	public void Enable(bool b)
	{
	}

	public void OnRestart()
	{
		LevelManager.m_CurLevel.Set(0, null, null, PvpJoinType.Invalid, 0);
	}

	public void OnApplicationQuit()
	{
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnApplicationPause(bool isPause)
	{
	}

	public bool IsLevelOpen(string battleId)
	{
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
		if (dataById == null)
		{
			return false;
		}
		bool flag = ToolsFacade.Instance.IsRuledDayOfWeek(dataById.scene_open_day);
		bool flag3;
		bool flag2 = ToolsFacade.Instance.IsInTimeInterval(dataById.scene_open_time, out flag3);
		bool flag4 = ToolsFacade.Instance.IsRuledDayOfWeek_Tomorrow(dataById.scene_open_day);
		return (!flag && flag2 && flag3 && flag4) || (flag && flag2 && !flag3);
	}

	public bool CheckSceneIsUnLock(int sceneLimitIndex)
	{
		return true;
	}

	public bool CheckSceneIsTest()
	{
		return LevelManager.m_CurLevel.level_id == "10061";
	}

	public void CheckShowUnlockView(int level)
	{
	}

	public static List<EntityVo> GetHeroes(TeamType team)
	{
		return LevelManager.m_CurLevel.GetTeamHeroes(team);
	}
}
