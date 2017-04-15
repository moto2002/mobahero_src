using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros;
using Newbie;
using System;
using UnityEngine;

namespace HUD.Module
{
	public class FunctionBtnsModule : BaseModule
	{
		private GameObject mDragLock;

		private GameObject mSkillInfo;

		private GameObject mSkillInfoBg;

		private GameObject mScoreBoard;

		private GameObject mSetting;

		private GameObject mSkillAnchor;

		private UIGrid mSkillGird;

		private SkillShowItem mSkillItem;

		private TweenPosition tPos_DragLockBtn;

		private TweenPosition tPos_SkillInfo;

		private TweenPosition tPos_ScoreBoard;

		private TweenPosition tPos_Setting;

		private Units mPlayer;

		private bool mDragLockRecord;

		public FunctionBtnsModule()
		{
			this.module = EHUDModule.FunctionBtns;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/FunctionBtnsModule");
		}

		public override void Init()
		{
			base.Init();
			this.mDragLock = this.transform.FindChild("TopLeftAnchor/DragLock").gameObject;
			this.mSkillInfo = this.transform.FindChild("BottomRightAnchor/SkillInfo").gameObject;
			this.mScoreBoard = this.transform.FindChild("BottomRightAnchor/ScoreBoard").gameObject;
			this.mSkillInfoBg = this.transform.Find("SkillAnchor/SkillBg/BackBg").gameObject;
			this.mSetting = this.transform.FindChild("BottomRightAnchor/Setting").gameObject;
			this.mSkillAnchor = this.transform.FindChild("SkillAnchor").gameObject;
			this.mSkillGird = this.transform.FindChild("SkillAnchor/SkillBg/Grid").GetComponent<UIGrid>();
			this.mSkillItem = ResourceManager.LoadPath<SkillShowItem>("Prefab/UI/PlayHUD/SkillShowItem", null, 0);
			this.tPos_DragLockBtn = this.mDragLock.GetComponent<TweenPosition>();
			this.tPos_ScoreBoard = this.mScoreBoard.GetComponent<TweenPosition>();
			this.tPos_Setting = this.mSetting.GetComponent<TweenPosition>();
			this.tPos_SkillInfo = this.mSkillInfo.GetComponent<TweenPosition>();
			UIEventListener.Get(this.mDragLock).onClick = new UIEventListener.VoidDelegate(this.OnClickDragLock);
			UIEventListener.Get(this.mSkillInfo).onClick = new UIEventListener.VoidDelegate(this.OnClickSkillInfo);
			UIEventListener.Get(this.mSkillInfoBg).onClick = new UIEventListener.VoidDelegate(this.OnClickSkillInfo);
			UIEventListener.Get(this.mScoreBoard).onClick = new UIEventListener.VoidDelegate(this.OnClickScoreBoard);
			UIEventListener.Get(this.mSetting).onClick = new UIEventListener.VoidDelegate(this.OnClickSetting);
		}

		public override void onFlyOut()
		{
			this.tPos_SkillInfo.PlayForward();
			this.tPos_Setting.PlayForward();
			this.tPos_ScoreBoard.PlayForward();
			this.tPos_DragLockBtn.PlayForward();
		}

		public override void onFlyIn()
		{
			this.tPos_SkillInfo.PlayReverse();
			this.tPos_Setting.PlayReverse();
			this.tPos_ScoreBoard.PlayReverse();
			this.tPos_DragLockBtn.PlayReverse();
		}

		public override void HandleAfterOpenModule()
		{
			base.HandleAfterOpenModule();
			this.mPlayer = MapManager.Instance.GetPlayer();
			this.mDragLockRecord = (BattleCameraMgr.Instance._currenCameraControllerType == CameraControllerType.Center);
			this.SetLockViewIcon(BattleCameraMgr.Instance._currenCameraControllerType);
			if (LevelManager.m_CurLevel.Is3V3V3())
			{
				this.mScoreBoard.gameObject.SetActive(false);
			}
			else
			{
				this.mScoreBoard.gameObject.SetActive(true);
			}
		}

		public override void RegisterUpdateHandler()
		{
			base.RegisterUpdateHandler();
			MobaMessageManager.RegistMessage((ClientMsg)26021, new MobaMessageFunc(this.OnShopOpened));
			MobaMessageManager.RegistMessage((ClientMsg)26022, new MobaMessageFunc(this.OnShopClosed));
		}

		public override void CancelUpdateHandler()
		{
			base.CancelUpdateHandler();
			MobaMessageManager.UnRegistMessage((ClientMsg)26021, new MobaMessageFunc(this.OnShopOpened));
			MobaMessageManager.UnRegistMessage((ClientMsg)26022, new MobaMessageFunc(this.OnShopClosed));
		}

		private void OnShopOpened(MobaMessage msg)
		{
			if (!GlobalSettings.Instance.isLockView)
			{
				GlobalSettings.Instance.isLockView = true;
				BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.Center);
			}
		}

		private void OnShopClosed(MobaMessage msg)
		{
			if (this.mPlayer == null)
			{
				this.mPlayer = MapManager.Instance.GetPlayer();
			}
			else if (this.mPlayer.isLive)
			{
				GlobalSettings.Instance.isLockView = this.mDragLockRecord;
				BattleCameraMgr.Instance.ChangeCameraController((!this.mDragLockRecord) ? CameraControllerType.Free : CameraControllerType.Center);
			}
			else if (GlobalSettings.Instance.isLockView)
			{
				GlobalSettings.Instance.isLockView = false;
				BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.Free);
			}
		}

		public void SetLockViewIcon(CameraControllerType cameraControllerType)
		{
			if (this.mDragLock == null)
			{
				return;
			}
			if (cameraControllerType == CameraControllerType.Free)
			{
				GlobalSettings.Instance.isLockView = false;
				this.mDragLock.GetComponent<UIToggle>().value = true;
			}
			if (cameraControllerType == CameraControllerType.Center)
			{
				GlobalSettings.Instance.isLockView = true;
				this.mDragLock.GetComponent<UIToggle>().value = false;
			}
		}

		public void OnClickDragLock(GameObject obj = null)
		{
			bool value = this.mDragLock.GetComponent<UIToggle>().value;
			if (this.mPlayer == null)
			{
				this.mPlayer = MapManager.Instance.GetPlayer();
			}
			if (value)
			{
				this.mDragLockRecord = false;
				GlobalSettings.Instance.isLockView = false;
				BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.Free);
			}
			else if (this.mPlayer.isLive)
			{
				this.mDragLockRecord = true;
				GlobalSettings.Instance.isLockView = true;
				BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.Center);
			}
			else
			{
				this.mDragLockRecord = false;
				GlobalSettings.Instance.isLockView = false;
				this.mDragLock.GetComponent<UIToggle>().value = !value;
			}
			NewbieManager.Instance.TryHandleCamDragLock();
		}

		public void NewbieOnClickSkillInfo(GameObject obj)
		{
			if (this.mSkillAnchor != null && this.mSkillAnchor.gameObject != null)
			{
				this.OnClickSkillInfo(obj);
			}
		}

		public void NewbieForceHideSkillInfo()
		{
			if (this.mSkillAnchor == null || this.mSkillAnchor.gameObject == null)
			{
				return;
			}
			if (!this.mSkillAnchor.gameObject.activeInHierarchy)
			{
				return;
			}
			this.mSkillAnchor.gameObject.SetActive(false);
		}

		public void OnClickSkillInfo(GameObject obj = null)
		{
			this.mSkillAnchor.gameObject.SetActive(!this.mSkillAnchor.gameObject.activeInHierarchy);
			if (this.mSkillAnchor.gameObject.activeInHierarchy)
			{
				Singleton<SkillView>.Instance.SetLevelUpBtns(false);
			}
			else
			{
				Singleton<SkillView>.Instance.SetLevelUpBtns(true);
			}
			if (!this.mSkillAnchor.gameObject.activeInHierarchy)
			{
				return;
			}
			this.UpdateSkillStudy();
		}

		public void UpdateSkillStudy()
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(player.npc_id);
			string[] heroSkills = heroMainData.skill_id.Split(new char[]
			{
				','
			});
			GridHelper.FillGrid<SkillShowItem>(this.mSkillGird, this.mSkillItem, (heroSkills.Length <= 4) ? heroSkills.Length : 4, delegate(int idx, SkillShowItem comp)
			{
				SysSkillMainVo skillData = SkillUtility.GetSkillData(heroSkills[idx], -1, -1);
				Texture textue = null;
				string type = (skillData.skill_trigger != 3) ? "(主动)" : "(被动)";
				string unikey = skillData.skill_id + "_lv0" + ((player.getSkillById(skillData.skill_id).skillLevel != 0) ? player.getSkillById(skillData.skill_id).skillLevel : 1).ToString();
				SysSkillLevelupVo dataById = BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(unikey);
				comp.Init(textue, skillData.skill_name, type, SkillView.FixSkillTxtInfo(BaseDataMgr.instance.GetLanguageData(dataById.skill_description).content, player), !player.skillManager.IsSkillUnlock(skillData.skill_id), (dataById.cd / (1f + player.GetAttr(AttrType.NormalSkillCooling))).ToString("0.0"));
			});
			this.mSkillGird.Reposition();
		}

		public void OnClickScoreBoard(GameObject obj = null)
		{
			if (Singleton<StatisticView>.Instance != null && Singleton<StatisticView>.Instance.gameObject != null && Singleton<StatisticView>.Instance.gameObject.activeInHierarchy)
			{
				Singleton<StatisticView>.Instance.Close_Statistic(null);
			}
			else if (Singleton<StatisticView>.Instance != null && Singleton<StatisticView>.Instance.gameObject == null)
			{
				Singleton<StatisticView>.Instance.SetModel(true);
				CtrlManager.OpenWindow(WindowID.StatisticView, null);
			}
		}

		public void OnClickSetting(GameObject obj = null)
		{
			CtrlManager.OpenWindow(WindowID.ReturnView, null);
			if (!LevelManager.Instance.IsPvpBattleType)
			{
				GameManager.SetGameState(GameState.Game_Pausing);
			}
			NewbieManager.Instance.TryHandleOpenSysSetting();
		}
	}
}
