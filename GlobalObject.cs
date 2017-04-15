using Assets.Scripts.Model;
using Assets.Scripts.Server;
using Com.Game.Module;
using GUIFramework;
using MobaClient;
using MobaHeros.Pvp;
using MobaHeros.Pvp.State;
using MobaProtocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GlobalObject : MonoBehaviour
{
	public static GlobalObject Instance;

	public GameManager m_GameManager;

	private float foretime;

	private float aftertime;

	private bool hasPaused;

	public bool forceLockReStart;

	private Dictionary<EGlobalComServer, IGlobalComServer> dicGlobalComServer;

	private bool canPause;

	private List<IGlobalComServer> listGlobalComServer;

	private GameObject JpushBinding;

	private void Awake()
	{
		if (GlobalObject.Instance == null)
		{
			GlobalObject.Instance = this;
		}
		PhotonClient.usePool = true;
		SerializeHelper.usePool = true;
		this.dicGlobalComServer = new Dictionary<EGlobalComServer, IGlobalComServer>
		{
			{
				EGlobalComServer.NetWorkHelper,
				new NetWorkHelper()
			},
			{
				EGlobalComServer.MVC_MessageManager,
				new MVC_MessageManager()
			},
			{
				EGlobalComServer.ModelManager,
				new ModelManager()
			},
			{
				EGlobalComServer.LoginStateManager,
				new LoginStateManager()
			},
			{
				EGlobalComServer.SceneManager,
				new SceneManager()
			},
			{
				EGlobalComServer.ViewMsgMonitor,
				new ViewMsgMonitor()
			},
			{
				EGlobalComServer.LevelManager,
				new LevelManager()
			},
			{
				EGlobalComServer.TestGCCollect,
				new TestGCCollect()
			},
			{
				EGlobalComServer.PvpStateManager,
				new PvpStateManager()
			},
			{
				EGlobalComServer.MobaMessageManager,
				new MobaMessageManager()
			},
			{
				EGlobalComServer.HomeManager,
				new HomeManager()
			},
			{
				EGlobalComServer.AnalyzeToolManager,
				new AnalyticsToolManager()
			},
			{
				EGlobalComServer.UIManager,
				new UIManager()
			},
			{
				EGlobalComServer.TaskManager,
				new TaskManager()
			},
			{
				EGlobalComServer.ResourceManager,
				ResourceManager.Instance
			},
			{
				EGlobalComServer.LogCallback,
				new LogCallback()
			},
			{
				EGlobalComServer.PvpMatchMgr,
				new PvpMatchMgr()
			},
			{
				EGlobalComServer.PlayerMng,
				new PlayerMng()
			},
			{
				EGlobalComServer.ToolsFacade,
				new ToolsFacade()
			}
		};
		this.Init();
		this.listGlobalComServer = new List<IGlobalComServer>();
		foreach (IGlobalComServer current in this.dicGlobalComServer.Values)
		{
			this.listGlobalComServer.Add(current);
		}
		for (int i = 0; i < this.listGlobalComServer.Count; i++)
		{
			this.listGlobalComServer[i].OnAwake();
		}
	}

	public void UpdateComps()
	{
		for (int i = 0; i < this.listGlobalComServer.Count; i++)
		{
			this.listGlobalComServer[i].OnUpdate();
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F12))
		{
			GlobalObject.ReStartGame();
		}
		if (Input.GetKeyUp(KeyCode.F11))
		{
			GlobalObject.QuitApp();
		}
		if (Time.frameCount % 20 == 0)
		{
		}
	}

	private void Start()
	{
		for (int i = 0; i < this.listGlobalComServer.Count; i++)
		{
			this.listGlobalComServer[i].OnStart();
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < this.listGlobalComServer.Count; i++)
		{
			this.listGlobalComServer[i].OnDestroy();
		}
	}

	private void OnApplicationPause(bool isPause)
	{
		if (isPause)
		{
			this.foretime = Time.realtimeSinceStartup;
			this.aftertime = this.foretime;
			this.hasPaused = true;
		}
		if (this.forceLockReStart)
		{
			this.hasPaused = false;
		}
		if (!isPause)
		{
			this.aftertime = Time.realtimeSinceStartup;
			if (this.hasPaused && this.aftertime - this.foretime > 299.9f)
			{
				this.hasPaused = false;
				this.Restart();
				return;
			}
			this.hasPaused = false;
		}
		if (this.forceLockReStart)
		{
			this.forceLockReStart = false;
		}
		if (isPause && this.canPause)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
		for (int i = 0; i < this.listGlobalComServer.Count; i++)
		{
			this.listGlobalComServer[i].OnApplicationPause(isPause);
		}
	}

	private void ConfirmRestart(bool isSure)
	{
		this.Restart();
	}

	public void SetCanPause(bool _canPause)
	{
		this.canPause = _canPause;
	}

	private void OnApplicationFocus(bool isFocus)
	{
		for (int i = 0; i < this.listGlobalComServer.Count; i++)
		{
			this.listGlobalComServer[i].OnApplicationFocus(isFocus);
		}
	}

	private void OnApplicationQuit()
	{
		for (int i = 0; i < this.listGlobalComServer.Count; i++)
		{
			this.listGlobalComServer[i].OnApplicationQuit();
		}
	}

	[Obsolete("反射产生临时变量，频繁调用，可能会导致GC")]
	private void InvokeEveryCom(string methodName, object[] param = null)
	{
		if (!string.IsNullOrEmpty(methodName))
		{
			for (int i = 0; i < this.listGlobalComServer.Count; i++)
			{
				IGlobalComServer globalComServer = this.listGlobalComServer[i];
				Type type = globalComServer.GetType();
				MethodInfo method = type.GetMethod(methodName);
				if (method != null)
				{
					method.Invoke(globalComServer, param);
				}
			}
		}
	}

	private void Init()
	{
		DebugView.InitLogger();
		Screen.sleepTimeout = -1;
		Application.runInBackground = true;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Debug.Log("Start SDKInit");
		if (GlobalSettings.isLoginByHoolaiSDK && !InitSDK.instance.IsInit())
		{
			InitSDK.instance.StartSDKInit();
		}
		if (GlobalSettings.isLoginByAnySDK && !InitSDK.instance.IsInit())
		{
			Debug.Log("StartAnySDKInit");
			InitSDK.instance.isFirstLogin = true;
			InitSDK.instance.StartAnySDKInit();
		}
	}

	private void Uninit()
	{
		CtrlManager.CloseWindow(WindowID.MenuTopBarView);
		CtrlManager.CloseWindow(WindowID.MenuView);
		CtrlManager.Clear();
		GlobalObject.ReSetGameSetting();
		UnityEngine.Object.DestroyImmediate(ViewTree.Instance.gameObject);
		UnityEngine.Object.DestroyImmediate(CameraRoot.Instance.gameObject);
		UnityEngine.Object.DestroyImmediate(base.gameObject);
		ViewTree.Instance = null;
		GlobalObject.Instance = null;
		GC.Collect();
		Resources.UnloadUnusedAssets();
	}

	private void Restart()
	{
		AudioMgr.unloadSoundBank_3V3V3();
		AudioMgr.unloadSoundBank_INGAME();
		AudioMgr.unloadSoundBank_UI();
		AudioMgr.loadSoundBank_UI();
		BgmPlayer.instance.StopBGM();
		this.ExitGame();
		for (int i = 0; i < this.listGlobalComServer.Count; i++)
		{
			this.listGlobalComServer[i].OnRestart();
		}
		this.Uninit();
		Application.LoadLevel("Start");
	}

	public static void QuitApp()
	{
		if (null != GlobalObject.Instance)
		{
			GlobalObject.Instance.OnApplicationQuit();
			GlobalObject.Instance.Uninit();
		}
		Application.Quit();
	}

	public static void ReStartGame()
	{
		if (null != GlobalObject.Instance)
		{
			GlobalObject.Instance.Restart();
		}
	}

	public void EnableGameManager(bool enabled)
	{
		if (this.m_GameManager != null)
		{
			this.m_GameManager.enabled = enabled;
		}
	}

	private static void ReSetGameSetting()
	{
		GlobalObject.Instance.m_GameManager = null;
	}

	public void ExitGame()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.GameExit_Immediately();
		}
	}

	private void LateUpdate()
	{
		ProtocolFactory.Clear();
	}
}
