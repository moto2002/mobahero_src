using Assets.Scripts.Model;
using GUIFramework;
using MobaHeros.Pvp;
using MobaHeros.Pvp.State;
using Newbie;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class ReturnView : BaseView<ReturnView>
	{
		public Transform ContinueButton;

		public Transform ExitButton;

		public Transform bgmButton;

		public Transform sfxButton;

		public UILabel sfxLabel;

		public UILabel bgmLabel;

		private ReturnController returnController;

		public UIToggle fpsToggle;

		public UIToggle fpsLimitToggle;

		private UIToggle mQuality_1080P;

		private UIPopupList mQualityList;

		private UIToggle cameraHeight;

		private Transform mAttackPriority;

		private UIToggle mChaOutline;

		private UIPopupList mChaOutlineList;

		private Transform mGameBgm;

		private Transform mGameSfx;

		private Transform mGameVoice;

		private Transform mGameCameraSpeed;

		private Transform mControlMode;

		private UIToggle mRecommend;

		private UIToggle mShopToggle;

		private GameObject mSkillPanelPivot_bottom;

		private GameObject mSkillPanelPivot_left;

		private GameObject mSkillPanelPivot_right;

		private bool haveexit;

		private CoroutineManager cMgr = new CoroutineManager();

		private string quality = string.Empty;

		private string levelStr = string.Empty;

		public ReturnView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "ReturnView");
		}

		public override void Init()
		{
			base.Init();
			this.returnController = this.transform.GetComponent<ReturnController>();
			this.mAttackPriority = this.transform.Find("ReturnPanel/NewPanel/Target");
			this.mChaOutline = this.transform.Find("ReturnPanel/NewPanel/Hero/Btn").GetComponent<UIToggle>();
			this.mChaOutlineList = this.transform.Find("ReturnPanel/NewPanel/OutlineList/Popup List").GetComponent<UIPopupList>();
			this.mGameBgm = this.transform.Find("ReturnPanel/NewPanel/Music");
			this.mGameSfx = this.transform.Find("ReturnPanel/NewPanel/Sound");
			this.mGameCameraSpeed = this.transform.Find("ReturnPanel/NewPanel/CameraSpeed");
			this.mRecommend = this.transform.Find("ReturnPanel/NewPanel/Recommend/Btn").GetComponent<UIToggle>();
			this.mShopToggle = this.transform.Find("ReturnPanel/NewPanel/Shop/Btn").GetComponent<UIToggle>();
			this.ExitButton = this.transform.Find("ReturnPanel/NewPanel/Surrender");
			this.ContinueButton = this.transform.Find("BackPanel/BackBtn");
			this.fpsToggle = this.transform.Find("ReturnPanel/NewPanel/FPS/Btn").GetComponent<UIToggle>();
			this.fpsLimitToggle = this.transform.Find("ReturnPanel/NewPanel/FPSLimit/Btn").GetComponent<UIToggle>();
			this.cameraHeight = this.transform.Find("ReturnPanel/NewPanel/Camera/Btn").GetComponent<UIToggle>();
			this.mControlMode = this.transform.Find("ReturnPanel/NewPanel/Skill");
			this.mQuality_1080P = this.transform.Find("ReturnPanel/NewPanel/Quality/Btn").GetComponent<UIToggle>();
			this.mQualityList = this.transform.Find("ReturnPanel/NewPanel/QualityList/Popup List").GetComponent<UIPopupList>();
			this.mSkillPanelPivot_bottom = this.transform.Find("ReturnPanel/NewPanel/SkillPanelPivot/BtnBelow").gameObject;
			this.mSkillPanelPivot_left = this.transform.Find("ReturnPanel/NewPanel/SkillPanelPivot/BtnLeft").gameObject;
			this.mSkillPanelPivot_right = this.transform.Find("ReturnPanel/NewPanel/SkillPanelPivot/BtnRight").gameObject;
			UIEventListener.Get(this.ExitButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.Exit);
			UIEventListener.Get(this.ContinueButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.Continue);
			UIEventListener.Get(this.fpsToggle.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnFPSBtn);
			UIEventListener.Get(this.fpsLimitToggle.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnFPSLiminiBtn);
			UIEventListener.Get(this.cameraHeight.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnCameraBtn);
			UIEventListener.Get(this.mRecommend.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnRecommendBtn);
			UIEventListener.Get(this.mShopToggle.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnShopToggle);
			UIEventListener.Get(this.mAttackPriority.FindChild("Btn").gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_AttackPriority);
			EventDelegate.Add(this.mChaOutline.onChange, new EventDelegate.Callback(this.OnChangeChaOutline));
			this.mChaOutline.value = GlobalSettings.Instance.isEnableChaOutline;
			EventDelegate.Add(this.mChaOutlineList.onChange, new EventDelegate.Callback(this.OnChangeChaOutlineList));
			if (GlobalSettings.Instance.ChaOutlineLevel == 0 && this.mChaOutlineList.value != "无")
			{
				this.mChaOutlineList.value = "无";
			}
			else if (GlobalSettings.Instance.ChaOutlineLevel == 1 && this.mChaOutlineList.value != "细")
			{
				this.mChaOutlineList.value = "细";
			}
			else if (GlobalSettings.Instance.ChaOutlineLevel == 2 && this.mChaOutlineList.value != "粗")
			{
				this.mChaOutlineList.value = "粗";
			}
			EventDelegate.Add(this.mQualityList.onChange, new EventDelegate.Callback(this.OnQualityBtn));
			UIEventListener.Get(this.mGameBgm.FindChild("Btn").gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_BgmToggle);
			UIEventListener.Get(this.mGameBgm.FindChild("Value/Slider/Thumb").gameObject).onDrag = new UIEventListener.VectorDelegate(this.onDrag_BgmProgBar);
			UIEventListener.Get(this.mGameSfx.FindChild("Btn").gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_SfxToggle);
			UIEventListener.Get(this.mGameSfx.FindChild("Value/Slider/Thumb").gameObject).onPress = new UIEventListener.BoolDelegate(this.onPress_SfxProgBar);
			UIEventListener.Get(this.mControlMode.FindChild("Btn").gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_ControlModeToggle);
			UIEventListener.Get(this.mSkillPanelPivot_bottom).onClick = new UIEventListener.VoidDelegate(this.onClick_SkillPanelPivot);
			UIEventListener.Get(this.mSkillPanelPivot_left).onClick = new UIEventListener.VoidDelegate(this.onClick_SkillPanelPivot);
			UIEventListener.Get(this.mSkillPanelPivot_right).onClick = new UIEventListener.VoidDelegate(this.onClick_SkillPanelPivot);
		}

		public override void RegisterUpdateHandler()
		{
			this.RefreshUI();
			if (!LevelManager.Instance.IsPvpBattleType)
			{
				GameManager.SetGameState(GameState.Game_Pausing);
			}
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			if (!Singleton<PvpManager>.Instance.IsObserver)
			{
				this.cMgr.StartCoroutine(this.SurrenderRefresh(), true);
			}
			base.HandleAfterOpenView();
			AudioMgr.Instance.PauseAll();
			this.RefreshBtnState();
			this.RefreshAudioToggle();
			this.RefreshCameraSlider();
			if (Singleton<SkillView>.Instance.gameObject)
			{
				Singleton<SkillView>.Instance.FlyOut();
			}
		}

		public override void HandleBeforeCloseView()
		{
			if (Singleton<SkillView>.Instance.gameObject)
			{
				Singleton<SkillView>.Instance.FlyIn();
			}
			this.cMgr.StopAllCoroutine();
			base.HandleBeforeCloseView();
		}

		public void TryShowSetSkillPivotHint()
		{
			if (this.transform == null)
			{
				return;
			}
			Transform transform = this.transform.Find("ReturnPanel/NewPanel/SkillPanelPivot/Label/SetHint");
			if (transform != null && transform.gameObject != null)
			{
				transform.gameObject.SetActive(true);
			}
		}

		private void TryHideSetSkillPivotHint()
		{
			if (this.transform == null)
			{
				return;
			}
			Transform transform = this.transform.Find("ReturnPanel/NewPanel/SkillPanelPivot/Label/SetHint");
			if (transform != null && transform.gameObject != null)
			{
				transform.gameObject.SetActive(false);
			}
		}

		public override void RefreshUI()
		{
			bool isPvpBattleType = LevelManager.Instance.IsPvpBattleType;
			string arg_3D_0 = (!Singleton<PvpManager>.Instance.IsObserver) ? LanguageManager.Instance.GetStringById("BattlePauseUI_Button_Continue") : LanguageManager.Instance.GetStringById("SpectatorPauseUI_Button_Continue");
			string text = string.Empty;
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				text = LanguageManager.Instance.GetStringById("SpectatorPauseUI_Button_Quit");
				this.ExitButton.GetComponent<UISprite>().enabled = true;
			}
			else if (isPvpBattleType)
			{
				text = LanguageManager.Instance.GetStringById("BattleSurrender_Button_Surrender");
			}
			else
			{
				text = LanguageManager.Instance.GetStringById("BattlePauseUI_Button_Quit");
			}
			this.ExitButton.Find("Label").GetComponent<UILabel>().text = text;
			this.returnController.StopAll();
			this.fpsToggle.value = GlobalSettings.Instance.isShowFPS;
			this.fpsToggle.startsActive = GlobalSettings.Instance.isShowFPS;
			this.fpsLimitToggle.value = GlobalSettings.Instance.isHighFPS;
			this.fpsLimitToggle.startsActive = GlobalSettings.Instance.isHighFPS;
			this.SetQualityValue();
			if (LevelManager.CurLevelId == "80005")
			{
				this.cameraHeight.transform.parent.gameObject.SetActive(false);
			}
		}

		public override void Destroy()
		{
			this.haveexit = false;
			base.Destroy();
		}

		private void RefreshBtnState()
		{
			this.mAttackPriority.FindChild("Btn").GetComponent<UIToggle>().value = (GlobalSettings.Instance.AttackSortType == SortType.Blood);
			this.cameraHeight.value = ModelManager.Instance.Get_SettingData().cameraHigh;
			this.mRecommend.value = ModelManager.Instance.Get_SettingData().recommendOn;
			this.mShopToggle.value = ModelManager.Instance.Get_SettingData().detailedShop;
			switch (ModelManager.Instance.Get_SettingData().skillPanelPivot)
			{
			case 0:
				this.mSkillPanelPivot_bottom.GetComponent<UIToggle>().value = true;
				break;
			case 1:
				this.mSkillPanelPivot_left.GetComponent<UIToggle>().value = true;
				break;
			case 2:
				this.mSkillPanelPivot_right.GetComponent<UIToggle>().value = true;
				break;
			}
		}

		[DebuggerHidden]
		private IEnumerator SurrenderRefresh()
		{
			ReturnView.<SurrenderRefresh>c__IteratorE5 <SurrenderRefresh>c__IteratorE = new ReturnView.<SurrenderRefresh>c__IteratorE5();
			<SurrenderRefresh>c__IteratorE.<>f__this = this;
			return <SurrenderRefresh>c__IteratorE;
		}

		private void Continue(GameObject objct = null)
		{
			this.RestoreSliderValue();
			ModelManager.Instance.Save_SettingData();
			this.returnController.WaitHide();
			HUDModuleMsgTools.SetSkillPanelPivot((SkillPanelPivot)ModelManager.Instance.Get_SettingData().skillPanelPivot);
			GameManager.SetGameState(GameState.Game_Resume);
			NewbieManager.Instance.TryHandleCloseSysSetting();
		}

		private void OnFPSBtn(GameObject objct = null)
		{
			GlobalSettings.Instance.isShowFPS = this.fpsToggle.value;
			ModelManager.Instance.Get_SettingData().showFPS = this.fpsToggle.value;
		}

		private void OnFPSLiminiBtn(GameObject objct = null)
		{
			GlobalSettings.Instance.isHighFPS = this.fpsLimitToggle.value;
			ModelManager.Instance.Get_SettingData().highFPS = this.fpsLimitToggle.value;
			if (GlobalSettings.Instance.isHighFPS)
			{
				CameraRoot.SetTargetFPS(60);
			}
			else
			{
				CameraRoot.SetTargetFPS(30);
			}
		}

		private void OnCameraBtn(GameObject objct = null)
		{
			ModelManager.Instance.Get_SettingData().cameraHigh = this.cameraHeight.value;
			if (this.cameraHeight.value)
			{
				BattleCameraMgr.Instance.ChangeCamera(2);
			}
			else
			{
				BattleCameraMgr.Instance.ChangeCamera(1);
			}
		}

		private void onClick_SkillPanelPivot(GameObject obj = null)
		{
			if (obj == this.mSkillPanelPivot_bottom)
			{
				ModelManager.Instance.Get_SettingData().skillPanelPivot = 0;
			}
			else if (obj == this.mSkillPanelPivot_left)
			{
				ModelManager.Instance.Get_SettingData().skillPanelPivot = 1;
			}
			else if (obj == this.mSkillPanelPivot_right)
			{
				ModelManager.Instance.Get_SettingData().skillPanelPivot = 2;
			}
			this.TryHideSetSkillPivotHint();
			NewbieManager.Instance.TryHandleSkillPanelSet();
		}

		private void OnRecommendBtn(GameObject obj = null)
		{
			bool value = this.mRecommend.value;
			MobaMessageManagerTools.BattleShop_recommendToggle(value);
			ModelManager.Instance.Get_SettingData().recommendOn = value;
		}

		private void OnShopToggle(GameObject obj = null)
		{
			bool value = this.mShopToggle.value;
			MobaMessageManagerTools.BattleShop_detailedShopToggle(value);
			ModelManager.Instance.Get_SettingData().detailedShop = value;
		}

		private void SetQualityValue()
		{
			switch (ModelManager.Instance.Get_SettingData().qualityLevel)
			{
			case MobaQualityLevel.Fast:
			case MobaQualityLevel.Hd:
				this.mQuality_1080P.value = false;
				this.mQualityList.value = "流畅";
				break;
			case MobaQualityLevel.SuperHd:
			case MobaQualityLevel.P1080:
				this.mQuality_1080P.value = true;
				this.mQualityList.value = "高清";
				break;
			case MobaQualityLevel.Original:
				this.mQualityList.value = "原画";
				break;
			}
		}

		private void OnQualityBtn()
		{
			if (this.quality == string.Empty)
			{
				this.quality = this.mQualityList.value;
				return;
			}
			if (this.quality == this.mQualityList.value)
			{
				return;
			}
			if (this.mQualityList.value == "流畅")
			{
				ModelManager.Instance.Set_Setting_QualityLevel(MobaQualityLevel.Hd);
			}
			else if (this.mQualityList.value == "高清")
			{
				ModelManager.Instance.Set_Setting_QualityLevel(MobaQualityLevel.P1080);
			}
			else if (this.mQualityList.value == "原画")
			{
				ModelManager.Instance.Set_Setting_QualityLevel(MobaQualityLevel.Original);
			}
			this.quality = this.mQualityList.value;
		}

		private void Exit(GameObject objct = null)
		{
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				CtrlManager.CloseWindow(WindowID.ReturnView);
				HomeGCManager.Instance.UnloadUISpriteAsset(this.ContinueButton.gameObject);
				PvpObserveMgr.QuitObserve();
				return;
			}
			if (LevelManager.Instance.IsPvpBattleType)
			{
				CtrlManager.CloseWindow(WindowID.ReturnView);
				GameManager.Instance.SurrenderMgr.StartSurrender();
				NewbieManager.Instance.TryHandleCloseSysSetting();
				return;
			}
			if (!this.haveexit)
			{
				this.haveexit = true;
				if (LevelManager.Instance.IsServerZyBattleType)
				{
					PvpStateManager.Instance.ChangeState(new PveStateInterrupt());
				}
				GameManager.SetGameState(GameState.Game_Exit);
				CtrlManager.CloseWindow(WindowID.ReturnView);
				HomeGCManager.Instance.UnloadUISpriteAsset(this.ContinueButton.gameObject);
			}
		}

		private void VolumeSwitchBG(GameObject objct)
		{
			if (AudioMgr.Instance.isBgMute())
			{
				this.bgmLabel.text = "音乐：开";
				AudioMgr.Instance.UnMuteBg();
			}
			else
			{
				this.bgmLabel.text = "音乐：关";
				AudioMgr.Instance.MuteBg();
			}
		}

		private void VolumeSwitchSFX(GameObject objct)
		{
			if (AudioMgr.Instance.isEffMute())
			{
				this.sfxLabel.text = "音效：开";
				AudioMgr.Instance.UnMuteEff();
			}
			else
			{
				this.sfxLabel.text = "音效：关";
				AudioMgr.Instance.MuteEff();
			}
		}

		private void RefreshAudioToggle()
		{
			UISlider component = this.mGameBgm.FindChild("Value/Slider").GetComponent<UISlider>();
			if (AudioMgr.Instance.isBgMute())
			{
				this.mGameBgm.FindChild("Btn").GetComponent<UIToggle>().value = false;
				component.GetComponent<BoxCollider>().enabled = false;
				component.thumb.GetComponent<BoxCollider>().enabled = false;
				component.value = 0f;
			}
			else
			{
				this.mGameBgm.FindChild("Btn").GetComponent<UIToggle>().value = true;
				component.GetComponent<BoxCollider>().enabled = true;
				component.thumb.GetComponent<BoxCollider>().enabled = true;
				this.mGameBgm.FindChild("Value/Slider").GetComponent<UISlider>().value = AudioMgr.getVolumeBG();
			}
			component = this.mGameSfx.FindChild("Value/Slider").GetComponent<UISlider>();
			if (AudioMgr.Instance.isEffMute())
			{
				this.mGameSfx.FindChild("Btn").GetComponent<UIToggle>().value = false;
				component.GetComponent<BoxCollider>().enabled = false;
				component.thumb.GetComponent<BoxCollider>().enabled = false;
				this.mGameSfx.FindChild("Value/Slider").GetComponent<UISlider>().value = 0f;
			}
			else
			{
				this.mGameSfx.FindChild("Btn").GetComponent<UIToggle>().value = true;
				component.GetComponent<BoxCollider>().enabled = true;
				component.thumb.GetComponent<BoxCollider>().enabled = true;
				this.mGameSfx.FindChild("Value/Slider").GetComponent<UISlider>().value = AudioMgr.getVolumeEff();
			}
			if (GlobalSettings.Instance.isCrazyMode)
			{
				this.mControlMode.FindChild("Btn").GetComponent<UIToggle>().value = true;
			}
			else
			{
				this.mControlMode.FindChild("Btn").GetComponent<UIToggle>().value = false;
			}
		}

		private void RefreshCameraSlider()
		{
			this.mGameCameraSpeed.FindChild("Value/Slider").GetComponent<UISlider>().value = BattleCameraMgr.Instance.GetSpeedSliderValue();
		}

		private void onClick_BgmToggle(GameObject obj = null)
		{
			UIToggle component = this.mGameBgm.FindChild("Btn").GetComponent<UIToggle>();
			UISlider component2 = this.mGameBgm.FindChild("Value/Slider").GetComponent<UISlider>();
			if (component.value)
			{
				AudioMgr.Instance.UnMuteBg();
				component2.GetComponent<BoxCollider>().enabled = true;
				component2.thumb.GetComponent<BoxCollider>().enabled = true;
				component2.value = AudioMgr.getVolumeBG();
			}
			else
			{
				AudioMgr.Instance.MuteBg();
				component2.GetComponent<BoxCollider>().enabled = false;
				component2.thumb.GetComponent<BoxCollider>().enabled = false;
				this.mGameBgm.FindChild("Value/Slider").GetComponent<UISlider>().value = 0f;
			}
		}

		private void onDrag_BgmProgBar(GameObject obj, Vector2 delta)
		{
			AudioMgr.setVolumeBG(this.mGameBgm.FindChild("Value/Slider").GetComponent<UISlider>().value, true);
		}

		private void onClick_SfxToggle(GameObject obj = null)
		{
			UIToggle component = this.mGameSfx.FindChild("Btn").GetComponent<UIToggle>();
			UISlider component2 = this.mGameSfx.FindChild("Value/Slider").GetComponent<UISlider>();
			if (component.value)
			{
				AudioMgr.Instance.UnMuteEff();
				component2.GetComponent<BoxCollider>().enabled = true;
				component2.thumb.GetComponent<BoxCollider>().enabled = true;
				this.mGameSfx.FindChild("Value/Slider").GetComponent<UISlider>().value = AudioMgr.getVolumeEff();
			}
			else
			{
				AudioMgr.Instance.MuteEff();
				component2.GetComponent<BoxCollider>().enabled = false;
				component2.thumb.GetComponent<BoxCollider>().enabled = false;
				this.mGameSfx.FindChild("Value/Slider").GetComponent<UISlider>().value = 0f;
			}
		}

		private void onPress_SfxProgBar(GameObject obj, bool isPress)
		{
			if (!isPress)
			{
				AudioMgr.PlayUI("Play_Menu_click", null, false, false);
				AudioMgr.setVolumeEff(this.mGameSfx.FindChild("Value/Slider").GetComponent<UISlider>().value);
			}
		}

		private void onClick_ControlModeToggle(GameObject obj = null)
		{
			UIToggle component = this.mControlMode.FindChild("Btn").GetComponent<UIToggle>();
			ModelManager.Instance.Get_SettingData().crazyCastingSkill = component.value;
			if (component.value)
			{
				PlayerPrefs.SetInt("IsCrazyMode", 1);
				GlobalSettings.Instance.isCrazyMode = true;
				PlayerControlMgr.Instance.GetPlayer().SetCrazyMode();
			}
			else
			{
				PlayerPrefs.SetInt("IsCrazyMode", 0);
				GlobalSettings.Instance.isCrazyMode = false;
				PlayerControlMgr.Instance.GetPlayer().SetNormalMode();
			}
			NewbieManager.Instance.TryHandleSetCastSkillMode(component.value);
		}

		private void onClick_AttackPriority(GameObject obj = null)
		{
			UIToggle component = this.mAttackPriority.FindChild("Btn").GetComponent<UIToggle>();
			if (component.value)
			{
				ModelManager.Instance.Get_SettingData().selectDyingHero = true;
				GlobalSettings.Instance.AttackSortType = SortType.Blood;
			}
			else
			{
				ModelManager.Instance.Get_SettingData().selectDyingHero = false;
				GlobalSettings.Instance.AttackSortType = SortType.Distance;
			}
		}

		private void OnChangeAttackPriority()
		{
			bool value = UIToggle.current.value;
			if (value)
			{
				ModelManager.Instance.Get_SettingData().selectDyingHero = true;
				GlobalSettings.Instance.AttackSortType = SortType.Blood;
			}
			else
			{
				ModelManager.Instance.Get_SettingData().selectDyingHero = false;
				GlobalSettings.Instance.AttackSortType = SortType.Distance;
			}
			PlayerPrefs.SetInt("AttackPriority", (int)GlobalSettings.Instance.AttackSortType);
		}

		private void OnChangeChaOutline()
		{
			bool value = UIToggle.current.value;
			ModelManager.Instance.Get_SettingData().characterOutline = value;
			GlobalSettings.Instance.isEnableChaOutline = value;
			PlayerPrefs.SetInt("EnableChaOutline", (!value) ? 0 : 1);
		}

		private void OnChangeChaOutlineList()
		{
			int num = 2;
			if (this.levelStr == string.Empty)
			{
				this.levelStr = this.mChaOutlineList.value;
				return;
			}
			if (this.levelStr == this.mChaOutlineList.value)
			{
				return;
			}
			if (this.mChaOutlineList.value == "无")
			{
				num = 0;
			}
			else if (this.mChaOutlineList.value == "细")
			{
				num = 1;
			}
			else if (this.mChaOutlineList.value == "粗")
			{
				num = 2;
			}
			this.levelStr = this.mChaOutlineList.value;
			ModelManager.Instance.Get_SettingData().characterOutlineList = (byte)num;
			GlobalSettings.Instance.ChaOutlineLevel = num;
			PlayerPrefs.SetInt("ChaOutlineLevel", num);
			PlayerPrefs.Save();
		}

		private void RestoreSliderValue()
		{
			float value = this.mGameBgm.FindChild("Value/Slider").GetComponent<UISlider>().value;
			if (!AudioMgr.Instance.isBgMute())
			{
				ModelManager.Instance.Get_SettingData().bgm = Convert.ToByte(value * 100f);
				AudioMgr.setVolumeBG(value, true);
			}
			else
			{
				ModelManager.Instance.Get_SettingData().bgm = 255;
			}
			value = this.mGameSfx.FindChild("Value/Slider").GetComponent<UISlider>().value;
			if (!AudioMgr.Instance.isEffMute())
			{
				ModelManager.Instance.Get_SettingData().sfx = Convert.ToByte(value * 100f);
				AudioMgr.setVolumeEff(value);
			}
			else
			{
				ModelManager.Instance.Get_SettingData().sfx = 255;
			}
			BattleCameraMgr.Instance.SetSpeedSliderValue(this.mGameCameraSpeed.FindChild("Value/Slider").GetComponent<UISlider>().value);
		}
	}
}
