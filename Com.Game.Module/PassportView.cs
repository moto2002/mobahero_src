using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using MobaServer;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace Com.Game.Module
{
	public class PassportView
	{
		private Transform transform;

		private UILabel NameLabel;

		private UILabel PlayerLevel;

		private UISprite PlayerExp;

		private UILabel PlayerID;

		private Transform Information_VIP;

		private UILabel NeedDiamond;

		private UILabel VIPLevel;

		private UISprite LevelNumber;

		private UISprite HeroHeadFrame_Texture;

		private UITexture HeroHead_Texture;

		private UITexture QRHeroHead;

		private UILabel Charming;

		private GameObject CharmRule;

		private UILabel MagicBottle;

		private UILabel MaxLadderScore;

		private UILabel NameCardCount;

		private UISprite whiteSprite;

		private Transform ChangeHead;

		private Transform ChangeHeadFrame;

		private Transform QuitSociaty;

		private UILabel Sociaty;

		private UILabel PlayerSociaty_Label;

		private UILabel PlayerSociaty_ID;

		private Transform ChangeName;

		private Transform SystemSetting;

		private Transform ReChange;

		private Transform SystemSettingPanel;

		private Transform SystemBackButton;

		private Transform SystemVoice;

		private UILabel SystemVoiceLabel;

		private Transform SystemRedeemCode;

		private Transform SystemBlackList;

		private Transform SystemPush;

		private Transform AccountUpgrade;

		private Transform AccountUpPanel;

		private Transform AccountUpPanel_No;

		private Transform AccountUpPanel_Yes;

		private UILabel SystemPushLabel;

		private Transform AllPopup_BG;

		private Transform AvatarNewMark;

		private Transform FrameNewMark;

		private Transform ChangeNamePanel;

		private Transform Dice;

		private Transform Name_Certain;

		private Transform Name_Cancel;

		private UIInput Name_Input;

		private Transform Name_Panel;

		private UITexture QRImage;

		private GameObject ShareQR;

		public Texture2D QRTexture;

		public PassportView(Transform trans)
		{
			this.transform = trans;
		}

		public void Init()
		{
			this.NameLabel = this.transform.Find("Label/NameLabel/Label").GetComponent<UILabel>();
			this.PlayerLevel = this.transform.Find("Label/PlayerLevel/Label").GetComponent<UILabel>();
			this.PlayerExp = this.transform.Find("Label/PlayerExp/Bar").GetComponent<UISprite>();
			this.PlayerID = this.transform.Find("Label/PlayerID/Label").GetComponent<UILabel>();
			this.Information_VIP = this.transform.Find("LevelBG/Information");
			this.NeedDiamond = this.transform.Find("LevelBG/Information/NeedDiamond").GetComponent<UILabel>();
			this.VIPLevel = this.transform.Find("LevelBG/Information/VIPLevel").GetComponent<UILabel>();
			this.LevelNumber = this.transform.Find("LevelBG/Grade/LevelNumber").GetComponent<UISprite>();
			this.Charming = this.transform.Find("Label/Charming/Label").GetComponent<UILabel>();
			this.CharmRule = this.transform.Find("Label/Charming/Rule").gameObject;
			this.MagicBottle = this.transform.Find("Label/MagicBottle/Label").GetComponent<UILabel>();
			this.MaxLadderScore = this.transform.Find("Label/LadderLv/Label").GetComponent<UILabel>();
			this.NameCardCount = this.transform.Find("NameCard").GetComponent<UILabel>();
			this.whiteSprite = this.transform.Find("HeroHeadFrame/White").GetComponent<UISprite>();
			this.HeroHeadFrame_Texture = this.transform.Find("HeroHeadFrame").GetComponent<UISprite>();
			this.HeroHead_Texture = this.transform.Find("HeroHeadFrame/HeroHead").GetComponent<UITexture>();
			this.QRHeroHead = this.transform.Find("QRImage/QRHeroHead").GetComponent<UITexture>();
			this.ChangeHead = this.transform.Find("ChangeHead");
			this.ChangeHeadFrame = this.transform.Find("ChangeHeadFrame");
			this.QuitSociaty = this.transform.Find("QuitSociaty");
			this.Sociaty = this.QuitSociaty.Find("Label").GetComponent<UILabel>();
			this.PlayerSociaty_Label = this.transform.Find("Label/PlayerSociaty/Label").GetComponent<UILabel>();
			this.PlayerSociaty_ID = this.transform.Find("Label/PlayerSociatyID/Label").GetComponent<UILabel>();
			this.ChangeName = this.transform.Find("ChangeName");
			this.SystemSetting = this.transform.Find("SystemSetting");
			this.ReChange = this.transform.Find("LevelBG/ReChange");
			this.SystemSettingPanel = this.transform.Find("AllPopup/SystemSettingPanel");
			this.SystemBackButton = this.SystemSettingPanel.Find("BackButton");
			this.SystemVoice = this.SystemSettingPanel.Find("Voice");
			this.SystemVoiceLabel = this.SystemVoice.Find("Label").GetComponent<UILabel>();
			this.SystemRedeemCode = this.SystemSettingPanel.Find("RedeemCode");
			this.SystemBlackList = this.SystemSettingPanel.Find("BlackList");
			this.SystemPush = this.SystemSettingPanel.Find("Push");
			this.AccountUpgrade = this.SystemSettingPanel.Find("AccountUpgrade");
			this.AccountUpPanel = this.SystemSettingPanel.Find("AccountUpPanel");
			this.AccountUpPanel_No = this.SystemSettingPanel.Find("AccountUpPanel/No");
			this.AccountUpPanel_Yes = this.SystemSettingPanel.Find("AccountUpPanel/Yes");
			this.SystemPushLabel = this.SystemPush.Find("Label").GetComponent<UILabel>();
			this.AllPopup_BG = this.transform.Find("AllPopup/BG");
			this.AvatarNewMark = this.ChangeHead.Find("NewMark");
			this.FrameNewMark = this.ChangeHeadFrame.Find("NewMark");
			this.ChangeNamePanel = this.transform.Find("AllPopup/ChangeNamePanel");
			this.Dice = this.ChangeNamePanel.Find("Dice");
			this.Name_Certain = this.ChangeNamePanel.Find("Certain");
			this.Name_Cancel = this.ChangeNamePanel.Find("Cancel");
			this.Name_Input = this.ChangeNamePanel.Find("Input").GetComponent<UIInput>();
			this.Name_Panel = this.ChangeNamePanel.Find("Panel");
			this.QRImage = this.transform.Find("QRImage").GetComponent<UITexture>();
			this.ShareQR = this.transform.Find("Share").gameObject;
			EventDelegate item = new EventDelegate(new EventDelegate.Callback(this.ChangeLabelText));
			this.Name_Input.GetComponent<UIInput>().onChange = new List<EventDelegate>();
			this.Name_Input.GetComponent<UIInput>().onChange.Add(item);
			UIEventListener.Get(this.ChangeName.gameObject).onClick = new UIEventListener.VoidDelegate(this.ChangeName_Event);
			UIEventListener.Get(this.Dice.gameObject).onClick = new UIEventListener.VoidDelegate(this.Dice_Event);
			UIEventListener.Get(this.Name_Certain.gameObject).onClick = new UIEventListener.VoidDelegate(this.Name_CertainOrCancel_Event);
			UIEventListener.Get(this.Name_Cancel.gameObject).onClick = new UIEventListener.VoidDelegate(this.Name_CertainOrCancel_Event);
			UIEventListener.Get(this.ChangeHead.gameObject).onClick = new UIEventListener.VoidDelegate(this.ChangeHead_Event);
			UIEventListener.Get(this.ChangeHeadFrame.gameObject).onClick = new UIEventListener.VoidDelegate(this.ChangeHeadFrame_Event);
			UIEventListener.Get(this.ReChange.gameObject).onClick = new UIEventListener.VoidDelegate(this.ReChange_Event);
			UIEventListener.Get(this.SystemBackButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.SystemBackButton_Event);
			UIEventListener.Get(this.AllPopup_BG.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_BG);
			UIEventListener.Get(this.ShareQR).onClick = new UIEventListener.VoidDelegate(this.Click_ShareQRCode);
			UIEventListener.Get(this.CharmRule).onClick = new UIEventListener.VoidDelegate(this.Click_CharmRule);
		}

		public void HandleAfterOpenView()
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "数据更新…", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetSummonersData, param, new object[0]);
		}

		public void HandleBeforeCloseView()
		{
			this.AllPopup_BG.gameObject.SetActive(false);
			this.ChangeNamePanel.gameObject.SetActive(false);
		}

		public void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.MagicBottleItem, new MobaMessageFunc(this.OnGetMsg_ModfiyNickName));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetSummonersData, new MobaMessageFunc(this.OnGetMsg_GetSummonersData));
			Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
		}

		public void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.MagicBottleItem, new MobaMessageFunc(this.OnGetMsg_ModfiyNickName));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetSummonersData, new MobaMessageFunc(this.OnGetMsg_GetSummonersData));
		}

		public void RefreshUI()
		{
			this.UpdatePassportData();
		}

		public void Destroy()
		{
		}

		private void UpdatePassportData()
		{
			this.InitPanel();
			int num = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
			int num2 = ModelManager.Instance.Get_userData_filed_X("Avatar");
			string text = ModelManager.Instance.Get_userData_filed_X("NickName");
			long num3 = ModelManager.Instance.Get_userData_filed_X("Exp");
			long num4 = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			int rank = ModelManager.Instance.Get_userData_filed_X("CharmRankValue");
			this.ShowVIPInformation();
			SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(num.ToString());
			if (dataById == null)
			{
				ClientLogger.Error("PictureFrame=" + num.ToString() + "SysSummonersPictureframeVo中找不到");
				this.HeroHeadFrame_Texture.spriteName = "pictureframe_0000";
			}
			else
			{
				this.HeroHeadFrame_Texture.spriteName = dataById.pictureframe_icon;
			}
			SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(num2.ToString());
			if (dataById2 == null)
			{
				ClientLogger.Error("Avatar=" + num2.ToString() + "SysSummonersHeadportraitVo中找不到");
				this.HeroHead_Texture.mainTexture = ResourceManager.Load<Texture>("headportrait_0001", true, true, null, 0, false);
				this.QRHeroHead.mainTexture = this.HeroHead_Texture.mainTexture;
			}
			else
			{
				this.HeroHead_Texture.mainTexture = ResourceManager.Load<Texture>(dataById2.headportrait_icon, true, true, null, 0, false);
				this.QRHeroHead.mainTexture = this.HeroHead_Texture.mainTexture;
			}
			this.NameLabel.text = text;
			this.NameLabel.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(rank);
			this.PlayerLevel.text = CharacterDataMgr.instance.GetUserLevel(num3).ToString();
			long exp = num3;
			this.PlayerExp.width = (int)((double)CharacterDataMgr.instance.GetUserCurrentExp(exp) / (double)CharacterDataMgr.instance.GetUserNextLevelExp(exp) * 220.0);
			this.PlayerID.text = num4.ToString();
			this.Charming.text = ModelManager.Instance.Get_userData_X().Charm.ToString();
			this.MagicBottle.text = ModelManager.Instance.Get_BottleData_Level().ToString();
			this.MaxLadderScore.text = ModelManager.Instance.Get_userData_X().HighScore.ToString();
			this.RefreshNameCardCount();
			this.CheckNewMark();
			this.GetQRTexture();
			this.UpdateSociatyView();
		}

		private void CheckNewMark()
		{
			List<string> portrait = CharacterDataMgr.instance.GetPortrait(CharacterDataMgr.Portrait.CommonPortrait);
			List<string> summonerFrame = CharacterDataMgr.instance.GetSummonerFrame();
			List<string[]> list = new List<string[]>();
			List<string[]> list2 = new List<string[]>();
			for (int i = 0; i < portrait.Count; i++)
			{
				string[] array = portrait[i].Split(new char[]
				{
					'_'
				});
				if (array.Length == 2)
				{
					list.Add(array);
				}
			}
			for (int j = 0; j < summonerFrame.Count; j++)
			{
				string[] array2 = summonerFrame[j].Split(new char[]
				{
					'_'
				});
				if (array2.Length == 2)
				{
					list2.Add(array2);
				}
			}
			if (list.Find((string[] item) => item[1] == "1") != null)
			{
				this.AvatarNewMark.gameObject.SetActive(true);
			}
			else
			{
				this.AvatarNewMark.gameObject.SetActive(false);
			}
			if (list2.Find((string[] item) => item[1] == "1") != null)
			{
				this.FrameNewMark.gameObject.SetActive(true);
			}
			else
			{
				this.FrameNewMark.gameObject.SetActive(false);
			}
		}

		public void SwitchAvatarNewMark(bool isNew)
		{
			this.AvatarNewMark.gameObject.SetActive(isNew);
		}

		public void SwitchFrameNewMark(bool isNew)
		{
			this.FrameNewMark.gameObject.SetActive(isNew);
		}

		private void InitPanel()
		{
			this.ChangeNamePanel.gameObject.SetActive(false);
			this.AllPopup_BG.gameObject.SetActive(false);
		}

		private void UpdateSociatyView()
		{
			int num = ModelManager.Instance.Get_userData_filed_X("UnionId");
			if (num > 0)
			{
				this.Sociaty.text = "退出战队";
				this.PlayerSociaty_Label.text = "小梦俱乐部";
				this.PlayerSociaty_ID.text = "2564700";
			}
			else
			{
				this.Sociaty.text = "加入战队";
				this.PlayerSociaty_Label.text = "无";
				this.PlayerSociaty_ID.text = "无";
			}
		}

		public void ShowVIPInformation()
		{
			int num = ModelManager.Instance.Get_userData_filed_X("VIP");
			int num2 = ModelManager.Instance.Get_userData_filed_X("PayMoneySum");
			int level = num;
			if (!CharacterDataMgr.instance.JudgeVIPFullLevel(level))
			{
				this.Information_VIP.gameObject.SetActive(true);
				this.VIPLevel.text = (num + 1).ToString();
				this.LevelNumber.spriteName = num.ToString();
				this.LevelNumber.width = this.LevelNumber.GetComponent<UISprite>().atlas.GetSprite(this.LevelNumber.spriteName).width;
				this.LevelNumber.height = this.LevelNumber.GetComponent<UISprite>().atlas.GetSprite(this.LevelNumber.spriteName).height;
				int number = num2;
				this.NeedDiamond.text = CharacterDataMgr.instance.GetVIPShow(number, 3).ToString();
			}
			else
			{
				this.Information_VIP.gameObject.SetActive(false);
				if (this.LevelNumber.atlas == null)
				{
					return;
				}
				this.LevelNumber.spriteName = num.ToString();
				this.LevelNumber.width = this.LevelNumber.GetComponent<UISprite>().atlas.GetSprite(this.LevelNumber.spriteName).width;
				this.LevelNumber.height = this.LevelNumber.GetComponent<UISprite>().atlas.GetSprite(this.LevelNumber.spriteName).height;
			}
		}

		private void ChangeName_Event(GameObject Object_1 = null)
		{
			this.Dice_Event(null);
			this.AllPopup_BG.gameObject.SetActive(true);
			this.ChangeNamePanel.gameObject.SetActive(true);
			this.ChangeNamePanel.gameObject.PlayShowOrHideAnim(true, null);
			this.Name_Panel.gameObject.SetActive(false);
		}

		private void Dice_Event(GameObject object_1 = null)
		{
			string[] array = GetRandomStr.Name(1);
			string value = array[0];
			this.Name_Input.value = value;
			this.Name_Input.GetComponent<UIInput>().value = value;
		}

		private void Name_CertainOrCancel_Event(GameObject object_1)
		{
			if (object_1 == this.Name_Cancel.gameObject)
			{
				this.Click_BG(null);
			}
			else
			{
				string value = this.Name_Input.value;
				if (string.IsNullOrEmpty(value))
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_NameCannotBeEmpty"), 1f);
					return;
				}
				if (value.Length > 7)
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_NameOverLength"), 1f);
				}
				else if (string.IsNullOrEmpty(value))
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_NameCannotBeEmpty"), 1f);
				}
				else
				{
					if (value.Contains("*"))
					{
						Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_IllegalCharacter"), 1f);
						return;
					}
					int num = ModelManager.Instance.Get_userData_filed_X("ChangNickNameCount");
					if (num < 1)
					{
						SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
						SendMsgManager.Instance.SendMsg(MobaGameCode.MagicBottleItem, param, new object[]
						{
							value
						});
					}
					else
					{
						CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("SummonerUI_Passport_ChangeNickname"), LanguageManager.Instance.GetStringById("SummonerUI_Passport_ChangeNicknameCost"), new Action<bool>(this.ChangeNameCallBack), PopViewType.PopTwoButton, "确定", "取消", null);
					}
				}
			}
		}

		private void OnGetMsg_ModfiyNickName(MobaMessage msg)
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
				if (mobaErrorCode != MobaErrorCode.NickNameExist)
				{
					this.ClickChangeName(num, operationResponse.DebugMessage, string.Empty);
				}
				else
				{
					this.ClickChangeName(num, operationResponse.DebugMessage, string.Empty);
				}
			}
			else
			{
				string k = (string)operationResponse.Parameters[232];
				this.ClickChangeName(num, operationResponse.DebugMessage, k);
			}
		}

		private void ClickChangeName(int i = 0, string j = null, string k = "")
		{
			if (i != 0)
			{
				if (i != 10106)
				{
					if (i != 70003)
					{
						Singleton<TipView>.Instance.ShowViewSetText("未知错误", 1f);
						this.RefreshUI();
						this.Name_Panel.gameObject.SetActive(false);
						this.Click_BG(null);
					}
					else
					{
						Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_MissingNameCard"), 1f);
					}
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_NameHasBeenUsed"), 1f);
				}
			}
			else
			{
				Singleton<MenuTopBarView>.Instance.RefreshUI();
				this.RefreshUI();
				this.Name_Panel.gameObject.SetActive(false);
				this.Click_BG(null);
			}
		}

		private void ChangeNameOrNot_Event(GameObject obj = null)
		{
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			if (list.Find((EquipmentInfoData data) => data.ModelId == 6666) == null)
			{
				CtrlManager.ShowMsgBox("提示", LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_MissingNameCard"), new Action<bool>(this.AddPassportCallback), PopViewType.PopTwoButton, "确定", "取消", null);
			}
			else if (list.Find((EquipmentInfoData data) => data.ModelId == 6666).Count >= 1)
			{
				string text = this.Name_Input.value.Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty);
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.MagicBottleItem, param, new object[]
				{
					text
				});
			}
		}

		private void RefreshNameCardCount()
		{
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			if (list != null)
			{
				int num = 0;
				if (list.Find((EquipmentInfoData data) => data.ModelId == 6666) != null)
				{
					num = list.Find((EquipmentInfoData data) => data.ModelId == 6666).Count;
				}
				this.NameCardCount.text = "x" + num.ToString();
			}
		}

		private void RechangeOrNot()
		{
			CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
			Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.RefreshNameCardCount));
			Singleton<PurchasePopupView>.Instance.Show(ETypicalCommodity.NameCard, false);
		}

		private void AddPassportCallback(bool isconfirm)
		{
			if (isconfirm)
			{
				this.RechangeOrNot();
			}
		}

		private void ChangeNameCallBack(bool isconfirm)
		{
			if (isconfirm)
			{
				this.ChangeNameOrNot_Event(null);
			}
		}

		private void ChangeHead_Event(GameObject Object_1 = null)
		{
			int frame = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
			int avatar = ModelManager.Instance.Get_userData_filed_X("Avatar");
			CtrlManager.OpenWindow(WindowID.ChangeAvatarView, null);
			Singleton<ChangeAvatarView>.Instance.ReFreshUI(true, avatar, frame);
		}

		private void ChangeHeadFrame_Event(GameObject Object_1 = null)
		{
			int frame = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
			int avatar = ModelManager.Instance.Get_userData_filed_X("Avatar");
			CtrlManager.OpenWindow(WindowID.ChangeAvatarView, null);
			Singleton<ChangeAvatarView>.Instance.ReFreshUI(false, avatar, frame);
		}

		private void OnGetMsg_GetSummonersData(MobaMessage msg)
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
			SummonerData summonerData = SerializeHelper.Deserialize<SummonerData>(operationResponse.Parameters[87] as byte[]);
			this.Charming.text = summonerData.Charm.ToString();
			this.MaxLadderScore.text = summonerData.HighScore.ToString();
		}

		private void ReChange_Event(GameObject Object_1 = null)
		{
			this.RechangeOrNot();
		}

		private void SystemBackButton_Event(GameObject Object_1 = null)
		{
			if (this.SystemSettingPanel.gameObject.activeInHierarchy)
			{
				this.SystemSettingPanel.gameObject.SetActive(false);
			}
			this.AllPopup_BG.gameObject.SetActive(false);
		}

		private void Click_CharmRule(GameObject obj = null)
		{
			CtrlManager.ShowMsgBox("如何获得魅力值?", "[EC3A76]魅力值[-]来自你获得与穿戴的皮肤、宠物、特效等，每赛季结束时结算排名，根据排名获得奖励。", delegate
			{
			}, PopViewType.PopOneButton, "我知道了", "取消", null);
		}

		private void Click_BG(GameObject object_1 = null)
		{
			if (this.AllPopup_BG.gameObject.activeInHierarchy)
			{
				if (this.SystemSettingPanel.gameObject.activeInHierarchy)
				{
					this.SystemSettingPanel.gameObject.PlayShowOrHideAnim(false, new EventDelegate.Callback(this.ClosePopup));
				}
				if (this.ChangeNamePanel.gameObject.activeInHierarchy)
				{
					this.ChangeNamePanel.gameObject.PlayShowOrHideAnim(false, new EventDelegate.Callback(this.ClosePopup));
				}
			}
		}

		private void ClosePopup()
		{
			if (this.SystemSettingPanel.gameObject.activeInHierarchy)
			{
				this.SystemSettingPanel.gameObject.SetActive(false);
			}
			if (this.ChangeNamePanel.gameObject.activeInHierarchy)
			{
				this.ChangeNamePanel.gameObject.SetActive(false);
			}
			this.AllPopup_BG.gameObject.SetActive(false);
		}

		private void GetQRTexture()
		{
			long num = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			if (this.QRTexture == null)
			{
				this.QRTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
				this.QRTexture.name = "PassportView_GetQRTexture_" + Time.time.ToString();
				string textForEncoding = num.ToString();
				Color32[] pixels = this.QREncode(textForEncoding, this.QRTexture.width, this.QRTexture.height);
				this.QRTexture.SetPixels32(pixels);
				for (int i = 0; i < this.QRTexture.width; i++)
				{
					for (int j = 0; j < this.QRTexture.height; j++)
					{
						if (this.QRTexture.GetPixel(i, j) == Color.white)
						{
							this.QRTexture.SetPixel(i, j, this.QRImage.color);
						}
						else if (this.QRTexture.GetPixel(i, j) == Color.black)
						{
							this.QRTexture.SetPixel(i, j, Color.clear);
						}
					}
				}
				this.QRTexture.Apply();
			}
			this.QRImage.mainTexture = this.QRTexture;
			this.QRImage.color = Color.white;
		}

		private Color32[] QREncode(string textForEncoding, int width, int height)
		{
			BarcodeWriter barcodeWriter = new BarcodeWriter
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Height = height,
					Width = width,
					Margin = 0,
					ErrorCorrection = ErrorCorrectionLevel.H
				}
			};
			return barcodeWriter.Write(textForEncoding);
		}

		private void Click_ShareQRCode(GameObject obj = null)
		{
			GameManager.Instance.DoShareSDK(2, new Rect((float)Screen.width * 0.247f, (float)Screen.height * 0.615f, 256f * (float)Screen.width / 1920f, 256f * (float)Screen.width / 1920f), this.whiteSprite);
		}

		private void ChangeLabelText()
		{
			UIFont component = this.Name_Input.transform.GetComponent<UIFont>();
			component.dynamicFont.RequestCharactersInTexture(this.Name_Input.value, 50);
			char[] array = this.Name_Input.value.Replace('\u3000', '\0').ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (!this.IsValid(component.dynamicFont, array[i]))
				{
					array[i] = '\0';
				}
			}
			string text = new string(array);
			text = text.Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty);
			text = FilterWorder.Instance.ReplaceKeyword(text);
			this.Name_Input.GetComponent<UIInput>().value = text;
		}

		private bool IsValid(Font font, char c)
		{
			CharacterInfo characterInfo;
			return font.GetCharacterInfo(c, out characterInfo, 50) && characterInfo.width > 0f;
		}
	}
}
