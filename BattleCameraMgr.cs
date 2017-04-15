using Com.Game.Module;
using Com.Game.Utils;
using HUD.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BattleCameraMgr : MonoBehaviour
{
	[SerializeField]
	private Transform _cameraRootTransform;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private Transform _listenerTrans;

	public AnimationCurve m_camMoveAsideCurve;

	public int m_nCurCameraID = 1;

	private Camera m_Camera2;

	private static BattleCameraMgr m_instance;

	private IConfineCamera _cameraConfiner;

	public float MinSpeed = 3.5f;

	public float MaxSpeed = 5.5f;

	public float ChangeXZTargetPosTimeDiff;

	public float speed = 0.12f;

	public float orignalSpeed = 0.12f;

	public Vector3 DiffOfCameraRootAndRole;

	public Vector3 DiffOfCameraRootAndRole1;

	public Vector3 DiffOfCameraRootAndRole2;

	private Dictionary<CameraControllerType, CameraControllerBase> _controllers = new Dictionary<CameraControllerType, CameraControllerBase>();

	public CameraControllerType _currenCameraControllerType;

	private CameraControllerType _lastCameraControllerType;

	public CameraControllerType _cameraControllerTypeBeforeDeath;

	public CoroutineManager corMg = new CoroutineManager();

	private CameraControllerBase _curCameraController;

	public float m_fScreenWidth;

	private VTrigger deathTrigger;

	private VTrigger tapMiniMapDownTrigger;

	private VTrigger tapMiniMapUpTrigger;

	private Units _watchTarget;

	public Transform ListenerTrans
	{
		get
		{
			return this._listenerTrans;
		}
	}

	public static BattleCameraMgr Instance
	{
		get
		{
			if (BattleCameraMgr.m_instance == null)
			{
			}
			return BattleCameraMgr.m_instance;
		}
	}

	public Camera BattleCamera
	{
		get
		{
			return this._camera;
		}
	}

	public Units WatchTarget
	{
		get
		{
			return this._watchTarget;
		}
	}

	private void InitCameraController()
	{
		if (this._controllers.Count != 0)
		{
			this._controllers.Clear();
			this._curCameraController = null;
			this._currenCameraControllerType = CameraControllerType.None;
		}
		this._controllers.Add(CameraControllerType.None, null);
		this._controllers.Add(CameraControllerType.Free, new CameraControllerFree(this, this._cameraRootTransform, this._camera));
		this._controllers.Add(CameraControllerType.Follow, new CameraControllerFollow(this, this._cameraRootTransform, this._camera));
		this._controllers.Add(CameraControllerType.Center, new CameraControllerCenter(this, this._cameraRootTransform, this._camera));
		this._controllers.Add(CameraControllerType.MoveByTap, new CameraControllerMoveByMapTap(this, this._cameraRootTransform, this._camera));
		this._controllers.Add(CameraControllerType.AlwaysFree, new CameraControllerAlwaysFree(this, this._cameraRootTransform, this._camera));
	}

	public void ChangeCameraController(CameraControllerType cameraControllerType)
	{
		if (GameManager.Instance.ReplayController.IsReplayStart)
		{
			cameraControllerType = CameraControllerType.Center;
		}
		if (cameraControllerType != this._currenCameraControllerType)
		{
			if (this._curCameraController != null)
			{
				this._curCameraController.OnExit();
			}
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (this._currenCameraControllerType != CameraControllerType.MoveByTap)
			{
				this._lastCameraControllerType = this._currenCameraControllerType;
			}
			if (this._cameraControllerTypeBeforeDeath == CameraControllerType.None)
			{
				this._cameraControllerTypeBeforeDeath = this._currenCameraControllerType;
			}
			this._currenCameraControllerType = cameraControllerType;
			this._curCameraController = this._controllers[this._currenCameraControllerType];
			this._curCameraController.OnEnter();
			if (Singleton<GoldView>.Instance.gameObject)
			{
				Singleton<GoldView>.Instance.SetLockViewIcon(cameraControllerType);
			}
			if (Singleton<HUDModuleManager>.Instance.gameObject)
			{
				FunctionBtnsModule module = Singleton<HUDModuleManager>.Instance.GetModule<FunctionBtnsModule>(EHUDModule.FunctionBtns);
				if (module != null)
				{
					module.SetLockViewIcon(cameraControllerType);
				}
				if (player != null && player.isLive)
				{
					this._cameraControllerTypeBeforeDeath = this._currenCameraControllerType;
				}
			}
		}
	}

	public CameraControllerType GetCameraControllerTypeBeforeDeath()
	{
		return this._cameraControllerTypeBeforeDeath;
	}

	public void DoRestoreCameraController()
	{
		if (this._currenCameraControllerType == this._lastCameraControllerType)
		{
			return;
		}
		if (this._curCameraController != null)
		{
			this._curCameraController.OnExit();
		}
		if (this._lastCameraControllerType != CameraControllerType.None && this._lastCameraControllerType != CameraControllerType.MoveByTap)
		{
			this._currenCameraControllerType = this._lastCameraControllerType;
		}
		this._curCameraController = this._controllers[this._currenCameraControllerType];
		this._curCameraController.OnEnter();
	}

	public CameraControllerBase GetCameraController(CameraControllerType cameraControllerType)
	{
		return this._controllers[cameraControllerType];
	}

	private void Awake()
	{
		BattleCameraMgr.m_instance = this;
		this.Register();
	}

	public void InitCameraParams()
	{
		CamRatio.SetupCamera(this._camera, 0f);
		GameObject gameObject = GameObject.FindGameObjectWithTag("SpawnPoint");
		CameraParams component = gameObject.GetComponent<CameraParams>();
		this._camera.fieldOfView = component.cameraFov;
		this._cameraConfiner = SimpleConfiner.Create(component);
	}

	public void ChangeCamera(int nIndex)
	{
		if (MapManager.Instance.MapPointContainer == null || MapManager.Instance.MapPointContainer.CameraSpawnPoints == null || this._cameraRootTransform == null)
		{
			return;
		}
		if (nIndex <= 0 || nIndex > MapManager.Instance.MapPointContainer.CameraSpawnPoints.Count)
		{
			return;
		}
		Transform transform = MapManager.Instance.MapPointContainer.CameraSpawnPoints[nIndex];
		if (transform == null)
		{
			return;
		}
		this.m_nCurCameraID = nIndex;
		this._cameraRootTransform.position = transform.position;
		this._camera.transform.localEulerAngles = transform.eulerAngles;
		if (nIndex == 1)
		{
			this.DiffOfCameraRootAndRole = this.DiffOfCameraRootAndRole1;
			this._cameraConfiner.ChangeRange(nIndex);
		}
		if (nIndex == 2)
		{
			this.DiffOfCameraRootAndRole = this.DiffOfCameraRootAndRole2;
			this._cameraConfiner.ChangeRange(nIndex);
		}
	}

	public void InitCamera2()
	{
		if (MapManager.Instance.MapPointContainer == null || MapManager.Instance.MapPointContainer.CameraSpawnPoints == null)
		{
			return;
		}
		Transform transform;
		if (!MapManager.Instance.MapPointContainer.CameraSpawnPoints.TryGetValue(2, out transform))
		{
			return;
		}
		if (transform == null)
		{
			return;
		}
		this.m_Camera2 = new Camera();
		this._cameraRootTransform.position = transform.position;
		this._cameraRootTransform.FindChild("XZOffsetNode").localPosition = Vector3.zero;
		this._camera.transform.localPosition = Vector3.zero;
		this._camera.transform.localEulerAngles = transform.eulerAngles;
		if (transform.camera == null)
		{
			return;
		}
		this._camera.farClipPlane = transform.camera.farClipPlane;
		this._camera.nearClipPlane = transform.camera.nearClipPlane;
		Vector3 scrPos = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.55f, 0f);
		Vector3 position = this._cameraRootTransform.transform.position;
		if (LevelManager.m_CurLevel.IsDaLuanDouPvp() && !Singleton<PvpManager>.Instance.IsObserver)
		{
			if (TeamManager.MyTeam == TeamType.LM)
			{
				scrPos = new Vector3((float)Screen.width * 0.4f, (float)Screen.height * 0.5f, 0f);
			}
			else
			{
				scrPos = new Vector3((float)Screen.width * 0.6f, (float)Screen.height * 0.6f, 0f);
			}
		}
		Vector3? vector = this.ScreenPos2WorldPos(scrPos);
		if (vector.HasValue)
		{
			this.DiffOfCameraRootAndRole2 = position - vector.Value;
		}
	}

	public void InitCamera()
	{
		this.InitCameraParams();
		this.InitCamera2();
		Transform transform = MapManager.Instance.MapPointContainer.CameraSpawnPoints[1];
		this._cameraRootTransform.position = transform.position;
		this._cameraRootTransform.FindChild("XZOffsetNode").localPosition = Vector3.zero;
		this._camera.transform.localPosition = Vector3.zero;
		this._camera.transform.localEulerAngles = transform.eulerAngles;
		if (PlayerPrefs.HasKey("CameraSpeed"))
		{
			this.speed = (0.5f + 1.5f * this.GetSpeedSliderValue()) * this.orignalSpeed;
		}
		this._camera.farClipPlane = transform.camera.farClipPlane;
		this._camera.nearClipPlane = transform.camera.nearClipPlane;
		this.CalcScreenWidth();
		Vector3 position = this._cameraRootTransform.transform.position;
		Vector3 scrPos = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.55f, 0f);
		if (LevelManager.m_CurLevel.IsDaLuanDouPvp() && !Singleton<PvpManager>.Instance.IsObserver)
		{
			if (TeamManager.MyTeam == TeamType.LM)
			{
				scrPos = new Vector3((float)Screen.width * 0.4f, (float)Screen.height * 0.5f, 0f);
			}
			else
			{
				scrPos = new Vector3((float)Screen.width * 0.6f, (float)Screen.height * 0.6f, 0f);
			}
		}
		Vector3? vector = this.ScreenPos2WorldPos(scrPos);
		if (vector.HasValue)
		{
			this.DiffOfCameraRootAndRole1 = position - vector.Value;
			this.DiffOfCameraRootAndRole = position - vector.Value;
		}
		else
		{
			ClientLogger.Error("cannot modify camera z, levelId = " + LevelManager.CurLevelId);
		}
		if (Math.Abs(this.DiffOfCameraRootAndRole.x) > 0.01f)
		{
		}
		this.InitCameraController();
		if (LevelManager.CurBattleType == 3)
		{
			this.ChangeCameraController(CameraControllerType.Center);
		}
		else if (Singleton<PvpManager>.Instance.IsObserver)
		{
			this.ChangeCameraController(CameraControllerType.AlwaysFree);
		}
		else
		{
			this.ChangeCameraController(CameraControllerType.Center);
		}
		this._camera.enabled = false;
		if (this.m_Camera2)
		{
			this.m_Camera2.enabled = false;
		}
	}

	public void ShowCamera()
	{
		if (!this._camera.enabled)
		{
			this._camera.enabled = true;
		}
		if (null != this.m_Camera2 && !this.m_Camera2.enabled)
		{
			this.m_Camera2.enabled = true;
		}
	}

	private void Update()
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.Update();
			if (Singleton<PvpManager>.Instance.IsObserver || this._curCameraController.GetType() == typeof(CameraControllerMoveByMapTap))
			{
				Units player = PlayerControlMgr.Instance.GetPlayer();
				if (player != null)
				{
					AudioMgr.enableListner(player.gameObject, false);
				}
			}
			else
			{
				Units player2 = PlayerControlMgr.Instance.GetPlayer();
				if (player2 != null)
				{
					AudioMgr.enableListner(player2.gameObject, true);
				}
			}
			if (GlobalSettings.Instance.isLockView && PlayerControlMgr.Instance != null && PlayerControlMgr.Instance.GetPlayer() != null)
			{
				AudioMgr.SetLisenerPos(PlayerControlMgr.Instance.GetPlayer().trans, Vector3.zero, 0);
			}
			else
			{
				AudioMgr.SetLisenerPos(this.ListenerTrans, Vector3.up, 0);
			}
		}
		if (this._cameraConfiner != null)
		{
			this._cameraConfiner.ConfineCamera(this._camera.transform);
		}
	}

	private Vector3? ScreenPos2WorldPos(Vector3 scrPos)
	{
		Ray ray = this._camera.ScreenPointToRay(scrPos);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
		{
			return new Vector3?(raycastHit.point);
		}
		return null;
	}

	public Vector3? GetScenePos()
	{
		return this.ScreenPos2WorldPos(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2)));
	}

	private void CalcScreenWidth()
	{
		Vector3? vector = this.ScreenPos2WorldPos(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
		if (!vector.HasValue)
		{
			vector = this.ScreenPos2WorldPos(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 4), 0f));
		}
		if (!vector.HasValue)
		{
			vector = this.ScreenPos2WorldPos(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 8), 0f));
		}
		Vector3? vector2 = this.ScreenPos2WorldPos(new Vector3((float)Screen.width, (float)(Screen.height / 2), 0f));
		if (!vector2.HasValue)
		{
			vector2 = this.ScreenPos2WorldPos(new Vector3((float)Screen.width, (float)(Screen.height / 4 * 3), 0f));
		}
		if (!vector2.HasValue)
		{
			vector2 = this.ScreenPos2WorldPos(new Vector3((float)Screen.width, (float)(Screen.height / 8 * 7), 0f));
		}
		if (vector.HasValue && vector2.HasValue)
		{
			this.m_fScreenWidth = Mathf.Abs(vector.Value.x - vector2.Value.x) * 2f;
		}
		else
		{
			ClientLogger.Error("m_fScreenWidth calc failed, levelId = " + LevelManager.CurLevelId);
		}
	}

	public void RegisterPlayerEvent()
	{
		Units player = PlayerControlMgr.Instance.GetPlayer();
		this._watchTarget = player;
		this.ReSetMiniMapTapEvent();
		if (player != null)
		{
			this.deathTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.PlayerDeath), player.unique_id);
			TriggerManager.CreateGameEventTrigger(GameEvent.ChangePlayer, null, new TriggerAction(this.ChangePlayer));
		}
		this.SetRoleObj(player, true);
	}

	private void ReSetMiniMapTapEvent()
	{
		if (this.tapMiniMapDownTrigger != null)
		{
			TriggerManager.DestroyTrigger(this.tapMiniMapDownTrigger);
			this.tapMiniMapDownTrigger = null;
		}
		if (this.tapMiniMapUpTrigger != null)
		{
			TriggerManager.DestroyTrigger(this.tapMiniMapUpTrigger);
			this.tapMiniMapUpTrigger = null;
		}
		if (this.tapMiniMapDownTrigger == null)
		{
			this.tapMiniMapDownTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.TapMiniMapDown, null, new TriggerAction(this.SetTouchMiniMapPosition));
		}
		if (this.tapMiniMapUpTrigger == null)
		{
			this.tapMiniMapUpTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.TapMiniMapUp, null, new TriggerAction(this.RestoreCameraController));
		}
	}

	private void Register()
	{
		MobaMessageManager.RegistMessage((ClientMsg)25039, new MobaMessageFunc(this.BattleEnd));
	}

	private void BattleEnd(MobaMessage msg)
	{
		if (LevelManager.m_CurLevel.battle_type == 9 || LevelManager.Instance.IsZyBattleType || LevelManager.m_CurLevel.battle_type == 2 || LevelManager.Instance.IsPvpBattleType)
		{
			TeamType teamType = (GameManager.FinalResult != TeamType.LM) ? TeamType.LM : TeamType.BL;
			Units home = MapManager.Instance.GetHome(teamType);
			if (home)
			{
				if (Singleton<PvpManager>.Instance.IsInPvp && home.isLive)
				{
					PvpProtocolTools.ToDie(home, null, 100L);
				}
				this.SetRoleObj(home, false);
			}
		}
	}

	private void UnRegister()
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)25039, new MobaMessageFunc(this.BattleEnd));
		if (this.tapMiniMapDownTrigger != null)
		{
			TriggerManager.DestroyTrigger(this.tapMiniMapDownTrigger);
			this.tapMiniMapDownTrigger = null;
		}
		if (this.tapMiniMapUpTrigger != null)
		{
			TriggerManager.DestroyTrigger(this.tapMiniMapUpTrigger);
			this.tapMiniMapUpTrigger = null;
		}
	}

	private void ChangePlayer()
	{
		if (this.deathTrigger != null)
		{
			TriggerManager.DestroyTrigger(this.deathTrigger);
			this.deathTrigger = null;
		}
		Units player = PlayerControlMgr.Instance.GetPlayer();
		this._watchTarget = player;
		if (player != null && player.isLive)
		{
			this.deathTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.PlayerDeath), player.unique_id);
			this.PlayerRespawn();
		}
		if (this.tapMiniMapDownTrigger == null)
		{
			this.tapMiniMapDownTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.TapMiniMapDown, null, new TriggerAction(this.SetTouchMiniMapPosition));
		}
		if (this.tapMiniMapUpTrigger == null)
		{
			this.tapMiniMapUpTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.TapMiniMapUp, null, new TriggerAction(this.RestoreCameraController));
		}
	}

	public void SetRoleObj(Units role, bool moveCameraImmediately = false)
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.SetRoleObj(role, moveCameraImmediately);
		}
	}

	public void SetTarget(Units target)
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.SetTarget(target);
		}
	}

	public void SetTapPositon(Units target)
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.SetTarget(target);
		}
	}

	public void PlayerDeath()
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.PlayerDeath();
		}
	}

	public void PlayerRespawn()
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.PlayerRespawn();
		}
	}

	public void SetTouchMiniMapPosition()
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.SetTouchMiniMapPosition();
		}
	}

	public void SetPostion(Vector3 v3)
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.SetPosition(v3);
		}
	}

	public void SetPositionAndMoveTime(Vector3 inPos, float inMoveTime)
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.SetPositionAndMoveTime(inPos, inMoveTime);
		}
	}

	public void RestoreCameraController()
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.RestoreCameraController();
		}
	}

	public void RestoreCameraController(float inMoveTime)
	{
		if (this._curCameraController != null)
		{
			this._curCameraController.RestoreCameraController(inMoveTime);
		}
	}

	public void ResetEvents()
	{
		this.ReSetMiniMapTapEvent();
	}

	public bool CanBattleCameraSee(Vector3 pos)
	{
		Vector3 vector = this._camera.WorldToScreenPoint(pos);
		return vector.x >= 0f && vector.x <= (float)Screen.width && vector.y >= 0f && vector.y <= (float)Screen.height;
	}

	public void SetSpeedSliderValue(float v)
	{
		this.speed = (0.5f + 1.5f * v) * this.orignalSpeed;
		PlayerPrefs.SetFloat("CameraSpeed", v);
		PlayerPrefs.Save();
	}

	public float GetSpeed()
	{
		return this.speed;
	}

	public float GetSpeedSliderValue()
	{
		if (PlayerPrefs.HasKey("CameraSpeed"))
		{
			return PlayerPrefs.GetFloat("CameraSpeed");
		}
		return 0.33f;
	}

	public void DelayChangeToFree()
	{
		this.corMg.StartCoroutine(this.WaitToFree(), true);
	}

	[DebuggerHidden]
	private IEnumerator WaitToFree()
	{
		BattleCameraMgr.<WaitToFree>c__Iterator17 <WaitToFree>c__Iterator = new BattleCameraMgr.<WaitToFree>c__Iterator17();
		<WaitToFree>c__Iterator.<>f__this = this;
		return <WaitToFree>c__Iterator;
	}
}
