using Assets.Scripts.Server;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public class SceneManager : IGlobalComServer
{
	private SceneType _preSceneType;

	private SceneType _curSceneType;

	private SceneType _nextSceneType;

	private SceneLoader curLoader;

	private SceneLoader preLoader;

	private SceneLoader nexLoader;

	private bool canChangeScene;

	private CoroutineManager m_CoroutineManager;

	private static SceneManager m_Instance;

	private List<string> list_unload;

	private List<string> list_load;

	private bool goOnFlag;

	public static SceneManager Instance
	{
		get
		{
			return SceneManager.m_Instance;
		}
	}

	public SceneType CurSceneType
	{
		get
		{
			return this._curSceneType;
		}
	}

	public bool GoOnFlag
	{
		get
		{
			return this.goOnFlag;
		}
		set
		{
			this.goOnFlag = value;
		}
	}

	public void OnAwake()
	{
		SceneManager.m_Instance = this;
		this.m_CoroutineManager = new CoroutineManager();
		this.list_unload = new List<string>();
		this.list_load = new List<string>();
		this.list_unload.Add("Unload");
		this.list_unload.Add("ClearViews");
		this.list_load.Add("BeginLoad");
		this.list_load.Add("LoadScene");
		this.list_load.Add("SetCamera");
		this.list_load.Add("SetFPS");
		this.list_load.Add("SetManager");
		this.list_load.Add("OpenViews");
		this.list_load.Add("LoadSceneEnd");
	}

	public void OnStart()
	{
	}

	public void OnDestroy()
	{
		this.m_CoroutineManager.StopAllCoroutine();
	}

	public void OnUpdate()
	{
		if (this.canChangeScene)
		{
			this.canChangeScene = false;
			if (this._preSceneType != this._curSceneType)
			{
				this._preSceneType = this._curSceneType;
			}
			this._curSceneType = this._nextSceneType;
			this.LoadScene();
		}
	}

	public void Enable(bool b)
	{
	}

	public void OnRestart()
	{
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

	private void LoadScene()
	{
		this.nexLoader = this.GetSceneLoader(this._curSceneType);
		this.m_CoroutineManager.StopAllCoroutine();
		this.m_CoroutineManager.StartCoroutine(this.LoadScene_Coroutine2(), true);
	}

	private SceneLoader GetSceneLoader(SceneType scene)
	{
		SceneLoader result = null;
		if (scene != SceneType.Login)
		{
			if (scene != SceneType.Home)
			{
				string battleSceneMapName = this.GetBattleSceneMapName();
				if (!string.IsNullOrEmpty(battleSceneMapName))
				{
					result = new SceneLoader_pvp(scene, this, battleSceneMapName);
				}
			}
			else if (this.curLoader != null && this.curLoader.SceneTypeVal == SceneType.Login)
			{
				result = new SceneLoader_home2(SceneType.Home, this, "Home");
			}
			else
			{
				result = new SceneLoader_home(SceneType.Home, this, "Home");
			}
		}
		else
		{
			result = new SceneLoader_login(SceneType.Login, this, "Login");
		}
		return result;
	}

	private string GetBattleSceneMapName()
	{
		string curLevelId = LevelManager.CurLevelId;
		if (string.IsNullOrEmpty(curLevelId))
		{
			UnityEngine.Debug.LogError("GetBattleSceneMapName empty or null, leevelId: " + LevelManager.CurLevelId);
			return string.Empty;
		}
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(curLevelId);
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysMapChangeVo>();
		string scene_map_id = dataById.scene_map_id;
		foreach (KeyValuePair<string, object> current in dicByType)
		{
			SysMapChangeVo sysMapChangeVo = current.Value as SysMapChangeVo;
			if (!(sysMapChangeVo.id != dataById.scene_map_id))
			{
				DateTime dateTime = ActivityTools.GetDateTime(sysMapChangeVo.start_time, true);
				DateTime dateTime2 = ActivityTools.GetDateTime(sysMapChangeVo.end_time, true);
				if (ToolsFacade.Instance.IsInTimeInterval(dateTime, dateTime2))
				{
					return sysMapChangeVo.id2;
				}
			}
		}
		return dataById.scene_map_id;
	}

	[DebuggerHidden]
	private IEnumerator LoadScene_Coroutine2()
	{
		SceneManager.<LoadScene_Coroutine2>c__Iterator1BA <LoadScene_Coroutine2>c__Iterator1BA = new SceneManager.<LoadScene_Coroutine2>c__Iterator1BA();
		<LoadScene_Coroutine2>c__Iterator1BA.<>f__this = this;
		return <LoadScene_Coroutine2>c__Iterator1BA;
	}

	private bool InvokeLoaderFunc(SceneLoader loader, string methodName)
	{
		bool result = false;
		if (loader != null && !string.IsNullOrEmpty(methodName))
		{
			Type type = loader.GetType();
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			if (method != null)
			{
				method.Invoke(loader, null);
				result = true;
			}
		}
		return result;
	}

	public void ChangeScene(SceneType scene, bool isloading = false)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25010, null, 0f);
		MobaMessageManager.ExecuteMsg(message);
		this._nextSceneType = scene;
		this.canChangeScene = true;
	}

	public void BackScene(bool isloading = false)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25010, null, 0f);
		MobaMessageManager.ExecuteMsg(message);
		this._nextSceneType = this._preSceneType;
		this.canChangeScene = true;
	}

	public void StartScene()
	{
		this._preSceneType = SceneType.None;
		this._curSceneType = SceneType.Start;
		this._nextSceneType = SceneType.Start;
		this.ChangeScene(SceneType.Login, false);
	}
}
