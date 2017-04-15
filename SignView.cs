using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignView : BaseView<SignView>
{
	private List<EquipmentInfoData> eids;

	private Transform RewardBtn;

	private Transform DisableBtn;

	private Transform CloseBtn;

	private Transform RewardList;

	private Transform RewardInfo;

	private UITexture RewardIcon;

	private UISprite Icon;

	public SignView()
	{
		this.WinResCfg = new WinResurceCfg(true, false, "Prefab/UI/CheckIn/SignView");
	}

	public override void Init()
	{
		base.Init();
		this.RewardBtn = this.transform.Find("GetRewardBtn");
		this.DisableBtn = this.transform.Find("DisableBtn");
		this.CloseBtn = this.transform.Find("CloseBtn");
		this.RewardList = this.transform.Find("RewardList");
		this.RewardInfo = this.transform.Find("InfoPanel");
		this.RewardIcon = this.transform.Find("InfoPanel/RewardIcon").GetComponent<UITexture>();
		this.Icon = this.transform.Find("InfoPanel/Icon").GetComponent<UISprite>();
		UIEventListener expr_BA = UIEventListener.Get(this.RewardBtn.gameObject);
		expr_BA.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_BA.onClick, new UIEventListener.VoidDelegate(this.ClickGetReward));
		UIEventListener expr_EB = UIEventListener.Get(this.CloseBtn.gameObject);
		expr_EB.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_EB.onClick, new UIEventListener.VoidDelegate(this.ClickClosePanel));
	}

	private void ClickGetReward(GameObject go)
	{
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
		SendMsgManager.Instance.SendMsg(MobaGameCode.SignDay, param, new object[0]);
	}

	private void ClickClosePanel(GameObject go)
	{
		CtrlManager.CloseWindow(WindowID.SignView);
	}

	private void ClosePanel()
	{
		CtrlManager.CloseWindow(WindowID.SignView);
		Singleton<MenuView>.Instance.RemoveNews(6, "0");
	}

	public override void HandleAfterOpenView()
	{
		AutoTestController.InvokeTestLogic(AutoTestTag.Home, delegate
		{
			this.ClickGetReward(null);
		}, 1f);
	}

	public override void HandleBeforeCloseView()
	{
		this.ClearResources();
	}

	private void ClearResources()
	{
		if (this.RewardIcon != null && this.RewardIcon.mainTexture != null)
		{
			this.RewardIcon.mainTexture = null;
		}
		if (this.RewardList != null)
		{
			this.ClearUITextureResources(this.RewardList.gameObject);
		}
	}

	private void ClearUITextureResources(GameObject inGo)
	{
		if (inGo == null)
		{
			return;
		}
		UITexture[] componentsInChildren = inGo.GetComponentsInChildren<UITexture>(true);
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UITexture uITexture = componentsInChildren[i];
				if (uITexture != null && uITexture.mainTexture != null)
				{
					uITexture.mainTexture = null;
				}
			}
		}
	}

	public override void RegisterUpdateHandler()
	{
		MVC_MessageManager.AddListener_view(MobaGameCode.SignDay, new MobaMessageFunc(this.OnGetMsg_SignDay));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetSignDay, new MobaMessageFunc(this.OnGetMsg_GetSignDay));
		this.RewardList.gameObject.SetActive(false);
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
		SendMsgManager.Instance.SendMsg(MobaGameCode.GetSignDay, param, new object[0]);
	}

	public override void CancelUpdateHandler()
	{
		MVC_MessageManager.RemoveListener_view(MobaGameCode.SignDay, new MobaMessageFunc(this.OnGetMsg_SignDay));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetSignDay, new MobaMessageFunc(this.OnGetMsg_GetSignDay));
	}

	public override void RefreshUI()
	{
	}

	private void OnGetMsg_SignDay(MobaMessage msg)
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
		if (mobaErrorCode == MobaErrorCode.Ok)
		{
			int checkCount = Convert.ToInt32(operationResponse.Parameters[101]);
			int week = (int)Convert.ToInt16(operationResponse.Parameters[10]);
			object obj = null;
			object obj2 = null;
			operationResponse.Parameters.TryGetValue(146, out obj);
			operationResponse.Parameters.TryGetValue(202, out obj2);
			if (obj2 != null)
			{
				this.eids = SerializeHelper.Deserialize<List<EquipmentInfoData>>(obj2 as byte[]);
			}
			if (obj != null)
			{
				List<DropItemData> list = SerializeHelper.Deserialize<List<DropItemData>>(obj as byte[]);
				this.GetReward(checkCount, list.Count > 0, week);
			}
			else
			{
				this.GetReward(checkCount, false, week);
			}
		}
	}

	private void OnGetMsg_GetSignDay(MobaMessage msg)
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
		if (mobaErrorCode == MobaErrorCode.Ok)
		{
			int isPass = Convert.ToInt32(operationResponse.Parameters[150]);
			int checkCount = Convert.ToInt32(operationResponse.Parameters[101]);
			int week = (int)Convert.ToInt16(operationResponse.Parameters[10]);
			this.UpdateRewardView(isPass, checkCount, week);
		}
	}

	private void GetReward(int checkCount, bool isRepeat, int week)
	{
		ModelManager.Instance.Get_GetSignDay_X().isPass = 0;
		this.RewardList.Find((checkCount + week * 7).ToString() + "/VMark").gameObject.SetActive(true);
		this.PlayDuang(checkCount + week * 7, isRepeat);
	}

	private void PlayDuang(int attendceId, bool isRepeat)
	{
		CtrlManager.OpenWindow(WindowID.GetItemsView, null);
		string[] rewardsInfo = this.ReturnRewardInfo(attendceId).Split(new char[]
		{
			'|'
		});
		if (rewardsInfo[0] == "1")
		{
			if (rewardsInfo[1] == "1")
			{
				ModelManager.Instance.Get_userData_X().Money += (long)int.Parse(rewardsInfo[2]);
				MobaMessageManagerTools.GetItems_Coin(int.Parse(rewardsInfo[2]));
			}
			else if (rewardsInfo[1] == "2")
			{
				ModelManager.Instance.Get_userData_X().Diamonds += (long)int.Parse(rewardsInfo[2]);
				MobaMessageManagerTools.GetItems_Diamond(int.Parse(rewardsInfo[2]));
			}
			else if (rewardsInfo[1] == "9")
			{
				ModelManager.Instance.Get_userData_X().SmallCap += int.Parse(rewardsInfo[2]);
				MobaMessageManagerTools.GetItems_Caps(int.Parse(rewardsInfo[2]));
			}
			else if (rewardsInfo[1] == "11")
			{
				ModelManager.Instance.Get_userData_X().Speaker += int.Parse(rewardsInfo[2]);
				MobaMessageManagerTools.GetItems_Speaker(int.Parse(rewardsInfo[2]));
			}
			Singleton<MenuTopBarView>.Instance.RefreshUI();
		}
		else if (rewardsInfo[0] == "2")
		{
			if (this.eids != null && this.eids.Count > 0)
			{
				foreach (EquipmentInfoData current in this.eids)
				{
					List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
					if (ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == int.Parse(rewardsInfo[1])) == null)
					{
						ModelManager.Instance.Get_equipmentList_X().Add(current);
					}
					else
					{
						ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == int.Parse(rewardsInfo[1])).Count += int.Parse(rewardsInfo[2]);
					}
				}
			}
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(rewardsInfo[1]);
			if (dataById.type == 4)
			{
				MobaMessageManagerTools.GetItems_Rune(int.Parse(rewardsInfo[1]));
			}
			else if (rewardsInfo[1] == "7777")
			{
				MobaMessageManagerTools.GetItems_Bottle(int.Parse(rewardsInfo[2]));
				Singleton<MenuView>.Instance.UpdateBottleNum();
			}
			else
			{
				MobaMessageManagerTools.GetItems_GameItem(rewardsInfo[1]);
			}
		}
		else if (rewardsInfo[0] == "3")
		{
			if (rewardsInfo[1] == "1")
			{
				Dictionary<string, SysHeroMainVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroMainVo>();
				string npc_id = typeDicByType.Values.FirstOrDefault((SysHeroMainVo obj) => obj.hero_id == int.Parse(rewardsInfo[2])).npc_id;
				if (isRepeat)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.Hero, npc_id, true);
				}
				else
				{
					MobaMessageManagerTools.GetItems_Hero(npc_id);
					CharacterDataMgr.instance.UpdateHerosData();
				}
			}
			else if (rewardsInfo[1] == "2")
			{
				if (isRepeat)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.HeroSkin, rewardsInfo[2], true);
				}
				else
				{
					MobaMessageManagerTools.GetItems_HeroSkin(int.Parse(rewardsInfo[2]));
				}
			}
			else if (rewardsInfo[1] == "3")
			{
				if (isRepeat)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.HeadPortrait, rewardsInfo[2], true);
				}
				else
				{
					MobaMessageManagerTools.GetItems_HeadPortrait(int.Parse(rewardsInfo[2]));
					ModelManager.Instance.GetNewAvatar(rewardsInfo[1], rewardsInfo[2]);
				}
			}
			else if (rewardsInfo[1] == "4")
			{
				if (isRepeat)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.PortraitFrame, rewardsInfo[2], true);
				}
				else
				{
					MobaMessageManagerTools.GetItems_PortraitFrame(rewardsInfo[2]);
					ModelManager.Instance.GetNewAvatar(rewardsInfo[1], rewardsInfo[2]);
				}
			}
			else if (rewardsInfo[1] == "5")
			{
				if (isRepeat)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.Coupon, rewardsInfo[2], true);
				}
				else
				{
					MobaMessageManagerTools.GetItems_Coupon(rewardsInfo[2]);
				}
			}
		}
		else if (rewardsInfo[0] == "6")
		{
			MobaMessageManagerTools.GetItems_GameBuff(Convert.ToInt32(rewardsInfo[1]));
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
		}
		else
		{
			if (rewardsInfo[1] == "1")
			{
				MobaMessageManagerTools.GetItems_Exp(int.Parse(rewardsInfo[2]), ModelManager.Instance.Get_userData_X().Exp);
				ModelManager.Instance.Get_userData_X().Exp += (long)int.Parse(rewardsInfo[2]);
				CharacterDataMgr.instance.SaveNowUserLevel(ModelManager.Instance.Get_userData_X().Exp);
				Singleton<MenuTopBarView>.Instance.RefreshUI();
			}
			Singleton<MenuTopBarView>.Instance.RefreshUI();
		}
		Singleton<GetItemsView>.Instance.onFinish = new Callback(this.ClosePanel);
		Singleton<GetItemsView>.Instance.Play();
	}

	private void UpdateRewardView(int isPass, int checkCount, int week)
	{
		this.RewardList.gameObject.SetActive(true);
		this.InitialSignItems(checkCount, week);
		for (int i = checkCount + 1; i <= this.RewardList.childCount; i++)
		{
			string text = (i + week * 7).ToString();
			this.RewardList.Find(text + "/VMark").gameObject.SetActive(false);
			this.RewardList.Find(text + "/Reward").GetComponent<UITexture>().alpha = 1f;
			this.RewardList.Find(text + "/Rune").GetComponent<UISprite>().alpha = 1f;
			this.RewardList.Find(text).GetComponent<UISprite>().spriteName = ((i != 7) ? "Checkins_bottom_blue" : "Checkins_bottom_red");
		}
		for (int j = 0; j < checkCount; j++)
		{
			string text2 = (j + week * 7 + 1).ToString();
			this.RewardList.Find(text2 + "/VMark").gameObject.SetActive(true);
			this.RewardList.Find(text2 + "/Reward").GetComponent<UITexture>().alpha = 0.5f;
			this.RewardList.Find(text2 + "/Rune").GetComponent<UISprite>().alpha = 0.5f;
			this.RewardList.Find(text2).GetComponent<UISprite>().spriteName = "Checkins_bottom_yellow";
		}
		bool flag = isPass == 0;
		this.RewardList.Find((checkCount + 1 + week * 7).ToString()).GetComponent<UISprite>().spriteName = "Checkins_bottom_yellow";
		this.DisableBtn.gameObject.SetActive(flag);
		this.RewardBtn.gameObject.SetActive(!flag);
		this.CloseBtn.gameObject.SetActive(flag);
	}

	private void InitialSignItems(int checkCount, int week)
	{
		for (int i = 1; i <= this.RewardList.childCount; i++)
		{
			int num = i + week * 7;
			this.RewardList.GetChild(i - 1).name = num.ToString();
			UIEventListener.Get(this.RewardList.Find(num.ToString()).gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowRewardInfo);
			this.RewardList.Find(num + "/Rune").gameObject.SetActive(false);
			this.RewardList.Find(num + "/Reward/Sprite").GetComponent<UISprite>().spriteName = "img_color";
			this.RewardList.Find(num + "/Reward/Sprite").GetComponent<UISprite>().color = new Color32(254, 185, 0, 255);
			SysAttendanceRewardsVo dataById = BaseDataMgr.instance.GetDataById<SysAttendanceRewardsVo>(num.ToString());
			SysGameResVo sysGameResVo = new SysGameResVo();
			if (dataById == null)
			{
				ClientLogger.Error(" Can't find id:" + num.ToString() + "in SysAttendanceRewardsVo");
			}
			else
			{
				if ("[]" != dataById.icon)
				{
					if (BaseDataMgr.instance.GetGameResData(dataById.icon) == null)
					{
						ClientLogger.Error(" Can't find id:" + dataById.icon + "in SysGameResVo");
					}
					else
					{
						this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById.icon, true, true, null, 0, false);
					}
				}
				SysDropRewardsVo dataById2 = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(dataById.rewards.ToString());
				SysDropItemsVo dataById3 = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(dataById2.drop_items);
				if (dataById3 == null)
				{
					ClientLogger.Error(" Can't find id:" + dataById2.drop_items + "in SysDropItemsVo");
				}
				else
				{
					string[] array = dataById3.rewards.Split(new char[]
					{
						'|'
					});
					string text = array[0];
					switch (text)
					{
					case "1":
					{
						this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().width = 206;
						this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().height = 206;
						this.RewardList.Find(num + "/Reward/Sprite").gameObject.SetActive(false);
						SysCurrencyVo dataById4 = BaseDataMgr.instance.GetDataById<SysCurrencyVo>(array[1]);
						if (dataById4 != null)
						{
							this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById4.name);
						}
						this.RewardList.Find(num + "/RewardCount").GetComponent<UILabel>().text = "x" + array[2];
						break;
					}
					case "2":
					{
						SysGameItemsVo dataById5 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(array[1]);
						this.RewardList.Find(num + "/Reward/Sprite").gameObject.SetActive(true);
						if (dataById5.type != 4 && dataById5.type != 10)
						{
							if (array[1] == "7777" || array[1] == "8000")
							{
								this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().width = 206;
								this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().height = 206;
								this.RewardList.Find(num + "/Reward/Sprite").gameObject.SetActive(false);
								this.RewardList.Find(num + "/RewardCount").GetComponent<UILabel>().text = "x" + array[2];
							}
							else
							{
								this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().width = 140;
								this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().height = 140;
							}
							this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById5.name);
						}
						else if (dataById5.type == 10)
						{
							this.RewardList.Find(num + "/Reward/Sprite").gameObject.SetActive(true);
							this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById5.role);
							this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().width = 140;
							this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().height = 140;
						}
						else
						{
							this.RewardList.Find(num + "/Rune").gameObject.SetActive(true);
							this.RewardList.Find(num + "/Rune").GetComponent<UISprite>().spriteName = dataById5.icon;
							this.RewardList.Find(num + "/Reward/Sprite").GetComponent<UISprite>().spriteName = "Checkins_icons_frame_0" + dataById5.quality;
							this.RewardList.Find(num + "/Reward/Sprite").GetComponent<UISprite>().color = Color.white;
							this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Rune");
							this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().width = 150;
							this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().height = 172;
						}
						this.RewardList.Find(num + "/RewardCount").GetComponent<UILabel>().text = "x" + array[2];
						break;
					}
					case "3":
					{
						this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().width = 140;
						this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().height = 140;
						this.RewardList.Find(num + "/Reward/Sprite").gameObject.SetActive(true);
						this.RewardList.Find(num + "/Reward/Sprite").GetComponent<UISprite>().spriteName = "Checkins_icons_frame_04";
						this.RewardList.Find(num + "/Reward/Sprite").GetComponent<UISprite>().color = Color.white;
						string text2 = array[1];
						switch (text2)
						{
						case "1":
							this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Hero");
							break;
						case "2":
							this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Skin");
							break;
						case "3":
							this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_HeadPortrait");
							break;
						case "4":
							this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_PictureFrame");
							break;
						}
						IL_9C5:
						this.RewardList.Find(num + "/RewardCount").GetComponent<UILabel>().text = "x1";
						break;
						goto IL_9C5;
					}
					case "4":
						this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().width = 123;
						this.RewardList.Find(num + "/Reward").GetComponent<UITexture>().height = 123;
						this.RewardList.Find(num + "/Reward/Sprite").gameObject.SetActive(false);
						this.RewardList.Find(num + "/RewardName").GetComponent<UILabel>().text = ((!(array[1] == "1")) ? "小魔瓶经验" : "召唤师经验");
						this.RewardList.Find(num + "/RewardCount").GetComponent<UILabel>().text = "x" + array[2];
						break;
					}
				}
			}
		}
	}

	private void ShowRewardInfo(GameObject go, bool isOver)
	{
		this.RewardInfo.gameObject.SetActive(isOver);
		this.RewardInfo.transform.localPosition = new Vector3(go.transform.localPosition.x + 452f, go.transform.localPosition.y - 204f, 0f);
		this.RewardIcon.gameObject.SetActive(true);
		this.Icon.gameObject.SetActive(false);
		SysAttendanceRewardsVo dataById = BaseDataMgr.instance.GetDataById<SysAttendanceRewardsVo>(go.name);
		SysDropRewardsVo dataById2 = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(dataById.rewards.ToString());
		SysDropItemsVo dataById3 = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(dataById2.drop_items);
		string[] array = dataById3.rewards.Split(new char[]
		{
			'|'
		});
		string text = array[0];
		switch (text)
		{
		case "1":
		{
			SysCurrencyVo dataById4 = BaseDataMgr.instance.GetDataById<SysCurrencyVo>(array[1]);
			if (dataById4 != null)
			{
				this.RewardInfo.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById4.name);
				this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById4.description);
				this.RewardIcon.mainTexture = ResourceManager.Load<Texture>(dataById.loading_icon, true, true, null, 0, false);
			}
			break;
		}
		case "2":
		{
			SysGameItemsVo dataById5 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(array[1]);
			if (dataById5.type != 4 && dataById5.type != 10)
			{
				this.RewardIcon.mainTexture = ResourceManager.Load<Texture>(dataById.loading_icon, true, true, null, 0, false);
				this.RewardInfo.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById5.name);
				this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById5.describe);
			}
			else if (dataById5.type == 10)
			{
				this.RewardIcon.mainTexture = ResourceManager.Load<Texture>(dataById.loading_icon, true, true, null, 0, false);
				this.RewardInfo.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById5.role);
				this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById5.name);
			}
			else
			{
				this.RewardIcon.gameObject.SetActive(false);
				this.Icon.gameObject.SetActive(true);
				this.Icon.transform.Find("Sprite").GetComponent<UISprite>().spriteName = dataById5.icon;
				this.RewardInfo.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Rune");
				this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById5.name);
			}
			break;
		}
		case "3":
		{
			string text2 = array[1];
			switch (text2)
			{
			case "1":
			{
				Dictionary<string, SysHeroMainVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroMainVo>();
				foreach (string current in typeDicByType.Keys)
				{
					if (typeDicByType[current].hero_id == int.Parse(array[2]))
					{
						SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(current);
						this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(heroMainData.name);
					}
				}
				this.RewardInfo.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Hero");
				break;
			}
			case "2":
			{
				SysHeroSkinVo dataById6 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(array[2]);
				this.RewardInfo.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Skin");
				this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById6.name);
				break;
			}
			case "3":
				this.RewardInfo.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_HeadPortrait");
				this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = string.Empty;
				break;
			case "4":
				this.RewardInfo.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_PictureFrame");
				this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = string.Empty;
				break;
			}
			this.RewardIcon.mainTexture = ResourceManager.Load<Texture>(dataById.loading_icon, true, true, null, 0, false);
			break;
		}
		case "4":
			this.RewardInfo.Find("Name").GetComponent<UILabel>().text = ((!(array[1] == "1")) ? "小魔瓶经验" : "召唤师经验");
			this.RewardIcon.mainTexture = ResourceManager.Load<Texture>(dataById.loading_icon, true, true, null, 0, false);
			this.RewardInfo.Find("Describe").GetComponent<UILabel>().text = string.Empty;
			break;
		}
	}

	private string ReturnRewardInfo(int index)
	{
		SysAttendanceRewardsVo dataById = BaseDataMgr.instance.GetDataById<SysAttendanceRewardsVo>(index.ToString());
		SysDropRewardsVo dataById2 = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(dataById.rewards.ToString());
		SysDropItemsVo dataById3 = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(dataById2.drop_items);
		return dataById3.rewards;
	}
}
