using Assets.Scripts.GUILogic.View.BattleEquipment;
using cn.sharesdk.unity3d;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using Mobaheros.AI.Equipment;
using MobaHeros.Pvp;
using MobaHeros.Replay;
using MobaHeros.Spawners;
using MobaTools.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public delegate void GameEventHandler(GameState oldState, GameState newState);

	private static GameManager _instance;

	private static GameState _gameState;

	private static GameState _preGameState;

	private bool _isEndGame;

	private readonly List<IGameModule> _gameModules = new List<IGameModule>();

	private readonly ShadowEffect _shadow = new ShadowEffect();

	private readonly StarManager _starManager = new StarManager();

	private readonly SkillDataPool _skillData = new SkillDataPool();

	private readonly DamageStatisticalManager _damageStatisticalManager = new DamageStatisticalManager();

	private readonly GameTimer _gameTimer = new GameTimer();

	private readonly GameEventModule _gameEventModule = new GameEventModule();

	private readonly PlayerControlMgr _playerControlMgr = new PlayerControlMgr();

	private readonly MapManager _mapManager = new MapManager();

	private readonly BattleShopCtrl _battleShopCtrl = new BattleShopCtrl();

	private readonly SurrenderMgr _surrenderMgr = new SurrenderMgr();

	private readonly ReplayController _replayController = new ReplayController();

	private readonly ChaosFightMgr _chaosFightMgr = new ChaosFightMgr();

	[SerializeField]
	private GameSpawner _spawner;

	[SerializeField]
	private AchieveManager _achieveManager;

	private ShareSDK ssdk;

	private int reqID;

	[SerializeField]
	private BattleRefresh _battleRefresh;

	private bool isreconected;

	private Camera _mainCamera;

	private Rect rect;

	private bool needCaptrue;

	private string picName;

	public event GameManager.GameEventHandler OnGameStateChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnGameStateChanged = (GameManager.GameEventHandler)Delegate.Combine(this.OnGameStateChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnGameStateChanged = (GameManager.GameEventHandler)Delegate.Remove(this.OnGameStateChanged, value);
		}
	}

	public static GameManager Instance
	{
		get
		{
			if (GameManager._instance == null)
			{
				GameManager._instance = UnityEngine.Object.FindObjectOfType<GameManager>();
			}
			return GameManager._instance;
		}
	}

	public GameSpawner Spawner
	{
		get
		{
			return this._spawner;
		}
	}

	public BattleRefresh BattleRefresh
	{
		get
		{
			return this._battleRefresh;
		}
	}

	public SkillDataPool SkillData
	{
		get
		{
			return this._skillData;
		}
	}

	public AchieveManager AchieveManager
	{
		get
		{
			return this._achieveManager;
		}
	}

	public DamageStatisticalManager DamageStatisticalManager
	{
		get
		{
			return this._damageStatisticalManager;
		}
	}

	public StarManager StarManager
	{
		get
		{
			return this._starManager;
		}
	}

	private static ShadowEffect Shadow
	{
		get
		{
			return GameManager.Instance._shadow;
		}
	}

	public PlayerControlMgr PlayerControlMgr
	{
		get
		{
			return this._playerControlMgr;
		}
	}

	public MapManager MapManager
	{
		get
		{
			return this._mapManager;
		}
	}

	public BattleShopCtrl BattleShopCtrlP
	{
		get
		{
			return this._battleShopCtrl;
		}
	}

	public SurrenderMgr SurrenderMgr
	{
		get
		{
			return this._surrenderMgr;
		}
	}

	public ChaosFightMgr ChaosFightMgr
	{
		get
		{
			return this._chaosFightMgr;
		}
	}

	public ReplayController ReplayController
	{
		get
		{
			return this._replayController;
		}
	}

	public static TeamType FinalResult
	{
		get
		{
			GameSpawner spawner = GameManager._instance._spawner;
			return (spawner != null) ? spawner.GameResult : TeamType.None;
		}
	}

	public static bool? IsVictory
	{
		get
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return GameManager.IsPvpWin;
			}
			TeamType finalResult = GameManager.FinalResult;
			if (finalResult == TeamType.None)
			{
				return null;
			}
			return new bool?(TeamType.LM == finalResult);
		}
	}

	private static bool? IsPvpWin
	{
		get
		{
			RoomInfo roomInfo = Singleton<PvpManager>.Instance.RoomInfo;
			if (roomInfo == null || !roomInfo.WinTeam.HasValue)
			{
				return null;
			}
			if (!Singleton<PvpManager>.Instance.IsObserver)
			{
				return new bool?(roomInfo.WinTeam.Value == Singleton<PvpManager>.Instance.SelfTeamType);
			}
			if (roomInfo.SummIdObserved != null)
			{
				return new bool?(roomInfo.WinTeam.Value == Singleton<PvpManager>.Instance.SelfTeamType);
			}
			return new bool?(true);
		}
	}

	public Camera mainCamera
	{
		get
		{
			if (this._mainCamera == null)
			{
				this._mainCamera = Camera.main;
			}
			return this._mainCamera;
		}
	}

	public static float TotalPlayingSeconds
	{
		get
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return Singleton<PvpManager>.Instance.TotalPlayingSeconds;
			}
			return GameManager._instance._gameTimer.TotalPlayingSeconds;
		}
	}

	public void initDrawer()
	{
	}

	public void ReConnected()
	{
		this.isreconected = true;
	}

	public static void SetGameState(GameState state)
	{
		GameManager._gameState = state;
	}

	public static bool IsPlaying()
	{
		return GameManager._gameState == GameState.Game_Playing || GameManager._gameState == GameState.Game_Resume;
	}

	public static bool IsPausing()
	{
		return GameManager._gameState == GameState.Game_Pausing;
	}

	public static bool IsGameOver()
	{
		return GameManager._gameState == GameState.Game_Over;
	}

	public static bool IsGameExit()
	{
		return GameManager._gameState == GameState.Game_Exit;
	}

	public static bool IsGameNone()
	{
		return GameManager._gameState == GameState.Game_None;
	}

	private void Awake()
	{
		GameManager._instance = this;
		if (this._battleRefresh == null)
		{
			this._battleRefresh = base.gameObject.AddComponent<BattleRefresh>();
		}
		FogMgr.Instance.Init();
		this._replayController.DoInition();
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		this.UpdateState();
		if (GameManager.IsPlaying())
		{
			FogMgr.Instance.Update();
			if (GlobalSettings.FogMode >= 2)
			{
				FOWSystem.Instance.DoUpdate();
			}
		}
	}

	private void OnApplicationFocus(bool isFocus)
	{
		if (!GameManager.IsPlaying())
		{
			return;
		}
		if (LevelManager.CurBattleType != 12)
		{
			CtrlManager.OpenWindow(WindowID.ReturnView, null);
			GameManager.SetGameState(GameState.Game_Pausing);
		}
	}

	public void StartGame()
	{
		GameManager._gameState = GameState.Game_None;
		GameManager._preGameState = GameState.Game_None;
		this._isEndGame = false;
		MobaMessageManager.RegistMessage((ClientMsg)25009, new MobaMessageFunc(this.OnSceneLoadComplete));
		GameManager.SetGameState(GameState.Game_Start);
		if (GlobalSettings.FogMode >= 2)
		{
			FOWSystem.Instance.Init();
			FOWSystem.Instance.BindCam(BattleCameraMgr.Instance.camera);
		}
		else
		{
			FOWSystem.Instance.Init();
		}
		this.initDrawer();
	}

	public void EndGame()
	{
		if (this._isEndGame)
		{
			return;
		}
		this._isEndGame = true;
		GameManager.ShowShadow(false);
		GameManager.SetGameState(GameState.Game_None);
		this.UninitAllModules();
		MobaMessageManager.UnRegistMessage((ClientMsg)25009, new MobaMessageFunc(this.OnSceneLoadComplete));
		if (GlobalSettings.FogMode >= 2 && FOWSystem.instance != null)
		{
			FOWSystem.Instance.OnDestroy();
		}
		HomeGCManager.Instance.ClearBattleResouces();
		RecieverObjCtrl.releaseAllTex();
	}

	private void OnSceneLoadComplete(MobaMessage msg)
	{
		GameManager.SetGameState(GameState.Game_Playing);
		AstarPath.active.enabled = false;
		if (GlobalSettings.FogMode >= 2)
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
			string scene_map_id = dataById.scene_map_id;
			FOWSystem.Instance.DoStart(scene_map_id);
		}
	}

	private void UpdateState()
	{
		if (GameManager._preGameState == GameManager._gameState)
		{
			return;
		}
		GameState preGameState = GameManager._preGameState;
		GameManager._preGameState = GameManager._gameState;
		switch (GameManager._gameState)
		{
		case GameState.Game_Start:
			base.StartCoroutine(this.GameStart());
			break;
		case GameState.Game_Playing:
			base.StartCoroutine(this.GamePlaying());
			break;
		case GameState.Game_Pausing:
			base.StartCoroutine(this.GamePausing());
			break;
		case GameState.Game_Resume:
			base.StartCoroutine(this.GameResume());
			break;
		case GameState.Game_Over:
			base.StartCoroutine(this.GameOver());
			break;
		case GameState.Game_Exit:
			base.StartCoroutine(this.GameExit());
			break;
		}
		foreach (IGameModule current in this._gameModules)
		{
			current.OnGameStateChange(preGameState, GameManager._gameState);
		}
		if (this.OnGameStateChanged != null)
		{
			this.OnGameStateChanged(preGameState, GameManager._gameState);
		}
	}

	[DebuggerHidden]
	private IEnumerator GameStart()
	{
		GameManager.<GameStart>c__Iterator1AA <GameStart>c__Iterator1AA = new GameManager.<GameStart>c__Iterator1AA();
		<GameStart>c__Iterator1AA.<>f__this = this;
		return <GameStart>c__Iterator1AA;
	}

	[DebuggerHidden]
	private IEnumerator GamePlaying()
	{
		GameManager.<GamePlaying>c__Iterator1AB <GamePlaying>c__Iterator1AB = new GameManager.<GamePlaying>c__Iterator1AB();
		<GamePlaying>c__Iterator1AB.<>f__this = this;
		return <GamePlaying>c__Iterator1AB;
	}

	[DebuggerHidden]
	private IEnumerator GamePausing()
	{
		return new GameManager.<GamePausing>c__Iterator1AC();
	}

	[DebuggerHidden]
	private IEnumerator GameResume()
	{
		return new GameManager.<GameResume>c__Iterator1AD();
	}

	[DebuggerHidden]
	private IEnumerator GameOver()
	{
		GameManager.<GameOver>c__Iterator1AE <GameOver>c__Iterator1AE = new GameManager.<GameOver>c__Iterator1AE();
		<GameOver>c__Iterator1AE.<>f__this = this;
		return <GameOver>c__Iterator1AE;
	}

	[DebuggerHidden]
	private IEnumerator GameExit()
	{
		GameManager.<GameExit>c__Iterator1AF <GameExit>c__Iterator1AF = new GameManager.<GameExit>c__Iterator1AF();
		<GameExit>c__Iterator1AF.<>f__this = this;
		return <GameExit>c__Iterator1AF;
	}

	public void GameExit_Immediately()
	{
		this.ResetData();
		Singleton<VTimer>.Instance.StopTimer();
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.GameExit);
		GameTimer.NormalTimeScale();
		CameraRoot.SetTargetFPS(30);
		TriggerManager.RemoveAllTriggers();
		this.EndGame();
	}

	private void RegisterModule(IGameModule module)
	{
		this._gameModules.Add(module);
	}

	private void InitAllModules()
	{
		this.RegisterModule(this._starManager);
		this.RegisterModule(this._skillData);
		this.RegisterModule(this._achieveManager);
		this.RegisterModule(this._damageStatisticalManager);
		this.RegisterModule(this._mapManager);
		this.RegisterModule(this._gameEventModule);
		this.RegisterModule(this._spawner);
		this.RegisterModule(this._playerControlMgr);
		this.RegisterModule(this._battleShopCtrl);
		this.RegisterModule(this._surrenderMgr);
		this.RegisterModule(this._chaosFightMgr);
		this.RegisterModule(this._replayController);
		foreach (IGameModule current in this._gameModules)
		{
			try
			{
				current.Init();
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}
	}

	private void UninitAllModules()
	{
		foreach (IGameModule current in this._gameModules)
		{
			try
			{
				current.Uninit();
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}
		this._gameModules.Clear();
	}

	private void CreateSpawner()
	{
		this._spawner = null;
		if (GlobalSettings.isTestModeOpen)
		{
			this._spawner = new TestSpawner();
		}
		else if (LevelManager.CurBattleType == 10)
		{
			ClientLogger.Error("TAP_TITAN is not available");
		}
		else if (LevelManager.Instance.IsPvpBattleType)
		{
			this._spawner = new PvpSpawner();
		}
		else if (LevelManager.Instance.IsServerZyBattleType)
		{
			this._spawner = new PveSpawner();
		}
		else
		{
			this._spawner = new GameSpawner();
		}
	}

	private void ResetData()
	{
		if (GameManager._gameState != GameState.Game_Over && GameManager._gameState != GameState.Game_Exit && GameManager._gameState != GameState.Game_None)
		{
			BattleAttrManager.Instance.Init();
			UtilManager.Instance.Init();
		}
		StrategyManager.Instance.Finish();
		AchieveData.Clear();
		AiEquipmentMgr.Instance.ResetAllData();
	}

	public static void ShowShadow(bool isshow)
	{
		if (GameManager.Shadow != null)
		{
			GameManager.Shadow.ShowShadows(isshow);
		}
	}

	public void DoShareSDK(int type, Rect _rect, UISprite spr = null)
	{
		NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
		if (this.ssdk == null)
		{
			this.ssdk = GlobalObject.Instance.transform.Find("Tools").GetComponent<ShareSDK>();
		}
		if (this.ssdk == null)
		{
			return;
		}
		this.ssdk.authHandler = new ShareSDK.EventHandler(this.AuthResultHandler);
		this.ssdk.shareHandler = new ShareSDK.EventHandler(this.ShareResultHandler);
		base.StartCoroutine(this.DoShareSDKTask(type, _rect, spr));
	}

	[DebuggerHidden]
	private IEnumerator DoShareSDKTask(int type, Rect _rect, UISprite spr = null)
	{
		GameManager.<DoShareSDKTask>c__Iterator1B0 <DoShareSDKTask>c__Iterator1B = new GameManager.<DoShareSDKTask>c__Iterator1B0();
		<DoShareSDKTask>c__Iterator1B.type = type;
		<DoShareSDKTask>c__Iterator1B.spr = spr;
		<DoShareSDKTask>c__Iterator1B._rect = _rect;
		<DoShareSDKTask>c__Iterator1B.<$>type = type;
		<DoShareSDKTask>c__Iterator1B.<$>spr = spr;
		<DoShareSDKTask>c__Iterator1B.<$>_rect = _rect;
		<DoShareSDKTask>c__Iterator1B.<>f__this = this;
		return <DoShareSDKTask>c__Iterator1B;
	}

	private void RenderPic(UISprite spr = null)
	{
		if (this.needCaptrue)
		{
			this.needCaptrue = false;
			Texture2D texture2D = new Texture2D((int)this.rect.width, (int)this.rect.height, TextureFormat.RGB24, false);
			texture2D.ReadPixels(this.rect, 0, 0);
			texture2D.Apply();
			if (spr != null)
			{
				spr.alpha = 0f;
			}
			byte[] bytes = texture2D.EncodeToPNG();
			UnityEngine.Object.Destroy(texture2D);
			string path = Application.persistentDataPath + "/" + this.picName + ".png";
			File.WriteAllBytes(path, bytes);
		}
	}

	private void CaptureScreenshot(Rect _rect, string _timer, UISprite spr = null)
	{
		this.rect = _rect;
		this.needCaptrue = true;
		this.picName = _timer;
		this.RenderPic(spr);
	}

	private void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			MonoBehaviour.print("authorize success !");
		}
		else if (state == ResponseState.Fail)
		{
			MonoBehaviour.print(string.Concat(new object[]
			{
				"fail! error code = ",
				result["error_code"],
				"; error msg = ",
				result["error_msg"]
			}));
			if (Application.platform == RuntimePlatform.Android)
			{
				Singleton<TipView>.Instance.ShowViewSetText("无可分享的社交平台，请安装！如：微信、QQ", 1f);
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("无可分享的社交平台，请安装！如：微信、QQ", 1f);
			}
		}
		else if (state == ResponseState.Cancel)
		{
			MonoBehaviour.print("cancel !");
		}
	}

	private void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			MonoBehaviour.print("share successfully - share result :");
			MonoBehaviour.print(MiniJSON.jsonEncode(result));
		}
		else if (state == ResponseState.Fail)
		{
			MonoBehaviour.print(string.Concat(new object[]
			{
				"fail! error code = ",
				result["error_code"],
				"; error msg = ",
				result["error_msg"]
			}));
			MonoBehaviour.print(string.Concat(new object[]
			{
				"fail! error code = ",
				result["error_code"],
				"; error msg = ",
				result["error_msg"]
			}));
			if (Application.platform == RuntimePlatform.Android)
			{
				Singleton<TipView>.Instance.ShowViewSetText("无可分享的社交平台，请安装！如：微信、QQ、新浪", 1f);
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("无可分享的社交平台，请安装！如：微信", 1f);
			}
		}
		else if (state == ResponseState.Cancel)
		{
			MonoBehaviour.print("cancel !");
		}
	}
}
