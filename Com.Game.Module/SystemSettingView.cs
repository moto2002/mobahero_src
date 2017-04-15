using anysdk;
using Assets.Scripts.Model;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaProtocol;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class SystemSettingView : BaseView<SystemSettingView>
	{
		private Transform SystemSettingPanel;

		private Transform SystemBackButton;

		private Transform SystemSound;

		private Transform SystemSfx;

		private Transform SystemVoice;

		private UIPopupList mQualityList;

		private UIToggle mQualityToggle;

		private GameObject mPush;

		private Transform SystemRedeemCode;

		private Transform RedeemCodePanel;

		private Transform RedeemCodePanelBackBtn;

		private Transform ChipCodeButton;

		private UIInput chipCodeInput;

		private Transform SystemBlackList;

		private UIToggle SystemPush;

		private Transform AccountUpgrade;

		private Transform AccountUpPanel;

		private Transform AccountUpPanel_No;

		private Transform AccountUpPanel_Yes;

		private GameObject ExitGameBtn;

		private GameObject LogoutBtn;

		private UISlider mBgVolumeSlider;

		private UISlider mEffVolumeSlider;

		private UISlider mVoiceVolumeSlider;

		private TweenAlpha mTAlpha;

		private TweenScale mTScale;

		private string quality = string.Empty;

		public SystemSettingView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/SystemSetting/SystemSettingView");
		}

		public override void Init()
		{
			base.Init();
			this.SystemSettingPanel = this.transform;
			this.SystemBackButton = this.SystemSettingPanel.Find("BackButton");
			this.SystemSound = this.SystemSettingPanel.Find("Music");
			this.SystemSfx = this.SystemSettingPanel.Find("Sound");
			this.SystemVoice = this.SystemSettingPanel.Find("Voice");
			this.mQualityToggle = this.transform.Find("Quality/Btn").GetComponent<UIToggle>();
			this.mQualityList = this.transform.Find("Quality/Popup List").GetComponent<UIPopupList>();
			this.mPush = this.transform.Find("Push/Btn").gameObject;
			this.SystemRedeemCode = this.SystemSettingPanel.Find("RedeemCode");
			this.RedeemCodePanel = this.SystemSettingPanel.Find("RedeemCodePanel");
			this.RedeemCodePanelBackBtn = this.RedeemCodePanel.Find("BackButton");
			this.ChipCodeButton = this.RedeemCodePanel.Find("FindButton");
			this.chipCodeInput = this.RedeemCodePanel.Find("Input").GetComponent<UIInput>();
			this.SystemBlackList = this.SystemSettingPanel.Find("BlackList");
			this.SystemPush = this.SystemSettingPanel.Find("Push/Btn").GetComponent<UIToggle>();
			this.AccountUpgrade = this.SystemSettingPanel.Find("AccountUpgrade");
			this.AccountUpPanel = this.SystemSettingPanel.Find("AccountUpPanel");
			this.AccountUpPanel_No = this.SystemSettingPanel.Find("AccountUpPanel/No");
			this.AccountUpPanel_Yes = this.SystemSettingPanel.Find("AccountUpPanel/Yes");
			this.ExitGameBtn = this.SystemSettingPanel.Find("ExitGame").gameObject;
			this.LogoutBtn = this.SystemSettingPanel.Find("Logout").gameObject;
			this.mBgVolumeSlider = this.SystemSound.Find("Value/Slider").GetComponent<UISlider>();
			this.mEffVolumeSlider = this.SystemSfx.Find("Value/Slider").GetComponent<UISlider>();
			this.mVoiceVolumeSlider = this.SystemVoice.Find("Value/Slider").GetComponent<UISlider>();
			this.mTAlpha = this.transform.GetComponent<TweenAlpha>();
			this.mTScale = this.transform.GetComponent<TweenScale>();
			UIEventListener.Get(this.mBgVolumeSlider.transform.FindChild("Thumb").gameObject).onDrag = new UIEventListener.VectorDelegate(this.UpdateBgVolume);
			UIEventListener.Get(this.mEffVolumeSlider.transform.FindChild("Thumb").gameObject).onPress = new UIEventListener.BoolDelegate(this.UpdateEffVolume);
			UIEventListener.Get(this.mVoiceVolumeSlider.transform.FindChild("Thumb").gameObject).onPress = new UIEventListener.BoolDelegate(this.UpdateVoiceVolume);
			UIEventListener.Get(this.mBgVolumeSlider.gameObject).onClick = new UIEventListener.VoidDelegate(this.UpdateBgVolumeClick);
			UIEventListener.Get(this.mEffVolumeSlider.gameObject).onClick = new UIEventListener.VoidDelegate(this.UpdateEffVolumeClick);
			UIEventListener.Get(this.mVoiceVolumeSlider.gameObject).onClick = new UIEventListener.VoidDelegate(this.UpdateVoiceVolumeClick);
			UIEventListener.Get(this.SystemBlackList.gameObject).onClick = new UIEventListener.VoidDelegate(this.SystemBlackList_Event);
			UIEventListener.Get(this.SystemRedeemCode.gameObject).onClick = new UIEventListener.VoidDelegate(this.SystemRedeemCode_Event);
			UIEventListener.Get(this.RedeemCodePanelBackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.HdieRedeemCodePanel);
			UIEventListener.Get(this.ChipCodeButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.ChipCodeButton_Event);
			UIEventListener.Get(this.SystemBackButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.SystemBackButton_Event);
			UIEventListener.Get(this.SystemSound.FindChild("Btn").gameObject).onClick = new UIEventListener.VoidDelegate(this.SystemSound_Event);
			UIEventListener.Get(this.SystemSfx.FindChild("Btn").gameObject).onClick = new UIEventListener.VoidDelegate(this.SystemSfx_Event);
			UIEventListener.Get(this.SystemVoice.FindChild("Btn").gameObject).onClick = new UIEventListener.VoidDelegate(this.SystemVoice_Event);
			UIEventListener.Get(this.SystemPush.gameObject).onClick = new UIEventListener.VoidDelegate(this.SystemPush_Event);
			UIEventListener.Get(this.AccountUpgrade.gameObject).onClick = new UIEventListener.VoidDelegate(this.AccountUpgrade_Event);
			UIEventListener.Get(this.AccountUpPanel_No.gameObject).onClick = new UIEventListener.VoidDelegate(this.AccountUpNoOrYes);
			UIEventListener.Get(this.AccountUpPanel_Yes.gameObject).onClick = new UIEventListener.VoidDelegate(this.AccountUpNoOrYes);
			UIEventListener.Get(this.ExitGameBtn).onClick = new UIEventListener.VoidDelegate(this.OnClickExitGame);
			UIEventListener.Get(this.LogoutBtn).onClick = new UIEventListener.VoidDelegate(this.OnClickLogout);
			EventDelegate.Add(this.mQualityList.onChange, new EventDelegate.Callback(this.OnQualityBtn));
			this.AnimationRoot = this.gameObject;
		}

		public override void HandleAfterOpenView()
		{
			this.ApplySetting();
			this.SetQualityValue();
			this.mTAlpha.ResetToBeginning();
			this.mTScale.ResetToBeginning();
			this.mTAlpha.PlayForward();
			this.mTScale.PlayForward();
		}

		public override void HandleBeforeCloseView()
		{
			this.RestoreSliderValue();
			ModelManager.Instance.Save_SettingData();
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaGameCode.ExchangeChip, new MobaMessageFunc(this.OnGetMsg_ExchangeChip));
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ExchangeChip, new MobaMessageFunc(this.OnGetMsg_ExchangeChip));
		}

		public override void RefreshUI()
		{
			this.UpdateSettingBtnState();
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void OnClickExitGame(GameObject obj = null)
		{
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				InitSDK.instance.SDKExit();
			}
			else if (GlobalSettings.isLoginByAnySDK)
			{
				InitSDK.instance.AnySDKExit(true);
			}
			else
			{
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("SystemSettingUI_Close01", "确认退出"), LanguageManager.Instance.GetStringById("SystemSettingUI_Close02", "确定退出刺激好玩的《魔霸英雄》吗？"), new Action<bool>(this.ExitCall), PopViewType.PopTwoButton, "确定", "取消", null);
			}
		}

		private void ExitCall(bool isConfirm)
		{
			if (isConfirm)
			{
				if (GlobalSettings.isLoginByAnySDK)
				{
					InitSDK.instance.SetAnySDKExtData("4");
					AnySDK.getInstance().release();
				}
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					GlobalObject.ReStartGame();
				}
				else
				{
					GlobalObject.QuitApp();
				}
			}
		}

		private void OnClickLogout(GameObject obj = null)
		{
			CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("SystemSettingUI_Exit01", "确认登出"), LanguageManager.Instance.GetStringById("SystemSettingUI_Exit02", "确定重启游戏以更换账号吗？"), new Action<bool>(this.LogoutCall), PopViewType.PopTwoButton, "确定", "取消", null);
		}

		private void LogoutCall(bool isConfirm)
		{
			if (isConfirm)
			{
				if (GlobalSettings.isLoginByHoolaiSDK)
				{
					InitSDK.instance.SDKLogout(true);
				}
				else if (GlobalSettings.isLoginByAnySDK)
				{
					InitSDK.instance.SDKAnySDKLogout(true);
				}
				else
				{
					GlobalObject.ReStartGame();
				}
			}
		}

		private void SetQualityValue()
		{
			SettingModelData settingModelData = ModelManager.Instance.Get_SettingData();
			MobaQualityLevel mobaQualityLevel;
			if (settingModelData != null)
			{
				mobaQualityLevel = ModelManager.Instance.Get_SettingData().qualityLevel;
			}
			else
			{
				mobaQualityLevel = MobaQualityLevel.P1080;
			}
			switch (mobaQualityLevel)
			{
			case MobaQualityLevel.Fast:
			case MobaQualityLevel.Hd:
				this.mQualityList.value = "流畅";
				break;
			case MobaQualityLevel.SuperHd:
			case MobaQualityLevel.P1080:
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

		private void TurnEff(bool turnOn)
		{
			if (turnOn)
			{
				this.SystemSfx.FindChild("Btn").GetComponent<UIToggle>().value = true;
				this.mEffVolumeSlider.GetComponent<BoxCollider>().enabled = true;
				this.mEffVolumeSlider.thumb.GetComponent<BoxCollider>().enabled = true;
				this.mEffVolumeSlider.value = AudioMgr.getVolumeEff();
			}
			else
			{
				this.SystemSfx.FindChild("Btn").GetComponent<UIToggle>().value = false;
				this.mEffVolumeSlider.GetComponent<BoxCollider>().enabled = false;
				this.mEffVolumeSlider.thumb.GetComponent<BoxCollider>().enabled = false;
				this.mEffVolumeSlider.value = 0f;
			}
		}

		private void TurnBgm(bool turnOn)
		{
			if (turnOn)
			{
				this.SystemSound.FindChild("Btn").GetComponent<UIToggle>().value = true;
				this.mBgVolumeSlider.GetComponent<BoxCollider>().enabled = true;
				this.mBgVolumeSlider.thumb.GetComponent<BoxCollider>().enabled = true;
				this.mBgVolumeSlider.value = AudioMgr.getVolumeBG();
			}
			else
			{
				this.SystemSound.FindChild("Btn").GetComponent<UIToggle>().value = false;
				this.mBgVolumeSlider.GetComponent<BoxCollider>().enabled = false;
				this.mBgVolumeSlider.thumb.GetComponent<BoxCollider>().enabled = false;
				this.mBgVolumeSlider.value = 0f;
			}
		}

		private void TurnVoice(bool turnOn)
		{
			if (turnOn)
			{
				this.SystemVoice.FindChild("Btn").GetComponent<UIToggle>().value = true;
				this.mVoiceVolumeSlider.GetComponent<BoxCollider>().enabled = true;
				this.mVoiceVolumeSlider.thumb.GetComponent<BoxCollider>().enabled = true;
				this.mVoiceVolumeSlider.value = AudioMgr.getVolumeVoice();
			}
			else
			{
				this.SystemVoice.FindChild("Btn").GetComponent<UIToggle>().value = false;
				this.mVoiceVolumeSlider.GetComponent<BoxCollider>().enabled = false;
				this.mVoiceVolumeSlider.thumb.GetComponent<BoxCollider>().enabled = false;
				this.mVoiceVolumeSlider.value = 0f;
			}
		}

		public void UpdateBgVolume(GameObject obj, Vector2 delta)
		{
			AudioMgr.setVolumeBG(this.mBgVolumeSlider.value, true);
		}

		public void UpdateBgVolumeClick(GameObject obj)
		{
			AudioMgr.setVolumeBG(this.mBgVolumeSlider.value, true);
		}

		public void UpdateEffVolume(GameObject obj, bool isPress)
		{
			if (!isPress)
			{
				AudioMgr.PlayUI("Play_Menu_click", null, false, false);
				AudioMgr.setVolumeEff(this.mEffVolumeSlider.value);
			}
		}

		public void UpdateEffVolumeClick(GameObject obj)
		{
			AudioMgr.PlayUI("Play_Menu_click", null, false, false);
			AudioMgr.setVolumeEff(this.mEffVolumeSlider.value);
		}

		public void UpdateVoiceVolume(GameObject obj, bool isPress)
		{
			if (!isPress)
			{
				AudioMgr.PlayUI("Play_Menu_click", null, false, false);
				AudioMgr.setVolumeVoice(this.mVoiceVolumeSlider.value);
			}
		}

		public void UpdateVoiceVolumeClick(GameObject obj)
		{
			AudioMgr.PlayUI("Play_Menu_click", null, false, false);
			AudioMgr.setVolumeVoice(this.mVoiceVolumeSlider.value);
		}

		private void UpdateSettingBtnState()
		{
			this.TurnBgm(!AudioMgr.Instance.isBgMute());
			this.TurnEff(!AudioMgr.Instance.isEffMute());
			this.TurnVoice(!AudioMgr.Instance.isVoiceMute());
			this.SystemPush.value = LocalNoti.CheckOpen();
		}

		private void SystemSound_Event(GameObject Object_1 = null)
		{
			if (AudioMgr.Instance.isBgMute())
			{
				AudioMgr.Instance.UnMuteBg();
				this.TurnBgm(true);
			}
			else
			{
				AudioMgr.Instance.MuteBg();
				this.TurnBgm(false);
			}
		}

		private void SystemSfx_Event(GameObject Object_1 = null)
		{
			if (AudioMgr.Instance.isEffMute())
			{
				AudioMgr.Instance.UnMuteEff();
				this.TurnEff(true);
			}
			else
			{
				AudioMgr.Instance.MuteEff();
				this.TurnEff(false);
			}
		}

		private void SystemVoice_Event(GameObject Object_1 = null)
		{
			if (AudioMgr.Instance.isVoiceMute())
			{
				AudioMgr.Instance.UnMuteVoice();
				this.TurnVoice(true);
			}
			else
			{
				AudioMgr.Instance.MuteVoice();
				this.TurnVoice(false);
			}
		}

		private void SystemRedeemCode_Event(GameObject Object_1 = null)
		{
			this.RedeemCodePanel.gameObject.SetActive(true);
		}

		private void HdieRedeemCodePanel(GameObject Object_1 = null)
		{
			this.RedeemCodePanel.gameObject.SetActive(false);
		}

		private void ChipCodeButton_Event(GameObject Object_1 = null)
		{
			string value = this.chipCodeInput.value;
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在兑换...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.ExchangeChip, param, new object[]
			{
				value
			});
			if (value == "paytestone")
			{
				InitSDK.instance.isPayTestOne = true;
			}
		}

		private void OnGetMsg_ExchangeChip(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode == MobaErrorCode.ChipError)
				{
					Singleton<TipView>.Instance.ShowViewSetText(SerializeHelper.Deserialize<string>((byte[])operationResponse.Parameters[221]), 1f);
				}
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText(SerializeHelper.Deserialize<string>((byte[])operationResponse.Parameters[221]), 1f);
				this.HdieRedeemCodePanel(null);
			}
		}

		private void SystemBlackList_Event(GameObject Object_1 = null)
		{
			CtrlManager.CloseWindow(WindowID.SystemSettingView);
			CtrlManager.OpenWindow(WindowID.BlackListView, null);
		}

		private void SystemPush_Event(GameObject Object_1 = null)
		{
			bool value = this.SystemPush.value;
			if (!SendMsgManager.Instance.SendMsg(MobaGameCode.SetPushState, null, new object[]
			{
				value
			}))
			{
				this.SystemPush.value = !value;
			}
			LocalNoti.SetOpenState(value);
		}

		private void AccountUpgrade_Event(GameObject object_1 = null)
		{
			this.AccountUpPanel.gameObject.SetActive(true);
			this.AccountUpPanel.gameObject.PlayShowOrHideAnim(true, null);
		}

		private void AccountUpNoOrYes(GameObject object_1 = null)
		{
			if (object_1 == this.AccountUpPanel_No.gameObject)
			{
				this.AccountUpPanel.gameObject.SetActive(false);
			}
			else if (object_1 == this.AccountUpPanel_Yes.gameObject)
			{
				Singleton<TipView>.Instance.ShowViewSetText("功能未开放", 1f);
				this.AccountUpPanel.gameObject.SetActive(false);
			}
		}

		private void SystemBackButton_Event(GameObject Object_1 = null)
		{
			CtrlManager.CloseWindow(WindowID.SystemSettingView);
		}

		private void ApplySetting()
		{
			ModelManager.Instance.Apply_SettingData();
		}

		private void RestoreSliderValue()
		{
			float value = this.mBgVolumeSlider.value;
			if (!AudioMgr.Instance.isBgMute())
			{
				ModelManager.Instance.Get_SettingData().bgm = Convert.ToByte(value * 100f);
				AudioMgr.setVolumeBG(value, true);
			}
			else
			{
				ModelManager.Instance.Get_SettingData().bgm = 255;
			}
			value = this.mEffVolumeSlider.value;
			if (!AudioMgr.Instance.isEffMute())
			{
				ModelManager.Instance.Get_SettingData().sfx = Convert.ToByte(value * 100f);
				AudioMgr.setVolumeEff(value);
			}
			else
			{
				ModelManager.Instance.Get_SettingData().sfx = 255;
			}
			value = this.mVoiceVolumeSlider.value;
			if (!AudioMgr.Instance.isVoiceMute())
			{
				ModelManager.Instance.Get_SettingData().voice = Convert.ToByte(value * 100f);
				AudioMgr.setVolumeVoice(value);
			}
			else
			{
				ModelManager.Instance.Get_SettingData().voice = 255;
			}
		}
	}
}
