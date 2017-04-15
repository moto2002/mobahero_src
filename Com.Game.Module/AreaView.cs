using Assets.Scripts.Model;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class AreaView : BaseView<AreaView>
	{
		private Transform mLeftAnchor;

		private Transform mServerList;

		private Transform mRightAnchor;

		private Transform mRecommendServerIcon;

		private UITexture mRecommendServerIcon_tex;

		private UILabel mRecommendServerIcon_lab;

		private Transform mLastSelectServerIcon;

		private UITexture mLastSelectServerIcon_tex;

		private UILabel mLastSelectServerIcon_lab;

		private UIButton mGoBackBtn;

		private UILabel mAccNO;

		private Transform mSelectedNow;

		private UITexture mSelectedNow_tex;

		private UILabel mSelectedNow_lab;

		private UISprite mSelectedNow_stateBall;

		private UILabel mSelectedNow_stateLabel;

		private AreaButtonCtrl mLastSelectServerBtn;

		private AreaButtonCtrl mRecommendServerBtn;

		private List<AreaButtonCtrl> mBtnGridControllers = new List<AreaButtonCtrl>();

		private GameObject mBtnCache;

		private UIButton mListBtn;

		private UIButton mConfirmBtn;

		private UILabel mConfirmLabel;

		private UIGrid mServerListGrid;

		private UIPanel mServerListScrollView;

		private UIAtlas atlas;

		private CoroutineManager cMgr;

		private int clickerCounter;

		private AreaInfo mSelectedArea;

		private bool _autoTest;

		private int fixTime;

		private bool sendMsgFlag = true;

		private bool refreshBtnStateFlag;

		private Task mServerMsgListenerTask;

		public AreaView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Login/AreaView");
		}

		public override void Init()
		{
			if (AutoTestController.IsEnable(AutoTestTag.SelectServer))
			{
				this._autoTest = true;
			}
			base.Init();
			this.BindObject();
			this.SetLanguage();
			this.InitBtnObject();
			this.SetRecommend();
			this.SetLastSelect();
			this.UploadDefaultSelect();
			this.SetCurrentSelect();
			this.SetAllBtnSelectState(this.mSelectedNow_lab.text);
			this.SetAccInfo();
		}

		protected override void BindObject()
		{
			this.mLeftAnchor = this.transform.Find("LeftAnchor");
			this.mServerList = this.transform.Find("ServerList");
			this.mRightAnchor = this.transform.Find("RightAnchor");
			this.mRecommendServerIcon = this.mLeftAnchor.FindChild("Recommend");
			this.mRecommendServerIcon_tex = this.mRecommendServerIcon.FindChild("iconMain").GetComponent<UITexture>();
			this.mRecommendServerIcon_lab = this.mRecommendServerIcon.FindChild("serverName").GetComponent<UILabel>();
			this.mLastSelectServerIcon = this.mLeftAnchor.FindChild("LastSelect");
			this.mLastSelectServerIcon_tex = this.mLastSelectServerIcon.FindChild("iconMain").GetComponent<UITexture>();
			this.mLastSelectServerIcon_lab = this.mLastSelectServerIcon.FindChild("serverName").GetComponent<UILabel>();
			this.mGoBackBtn = this.mLeftAnchor.FindChild("GoBack").GetComponent<UIButton>();
			this.mAccNO = this.mLeftAnchor.FindChild("AccountNO/ACCNO").GetComponent<UILabel>();
			this.mRecommendServerBtn = this.mServerList.FindChild("Scroll View/Fix_1/ServerBtn").GetComponent<AreaButtonCtrl>();
			this.mLastSelectServerBtn = this.mServerList.FindChild("Scroll View/Fix_2/ServerBtn").GetComponent<AreaButtonCtrl>();
			this.mServerListGrid = this.mServerList.FindChild("Scroll View/Grid").GetComponent<UIGrid>();
			this.mServerListScrollView = this.mServerList.FindChild("Scroll View").GetComponent<UIPanel>();
			this.mBtnCache = this.mServerListGrid.transform.FindChild("ServerBtnCache").gameObject;
			this.mSelectedNow = this.mRightAnchor.FindChild("MainIcon");
			this.mSelectedNow_tex = this.mSelectedNow.FindChild("iconMain").GetComponent<UITexture>();
			this.mSelectedNow_lab = this.mSelectedNow.FindChild("serverName").GetComponent<UILabel>();
			this.mSelectedNow_stateBall = this.mSelectedNow.FindChild("serverState/stateBall").GetComponent<UISprite>();
			this.mSelectedNow_stateLabel = this.mSelectedNow.FindChild("serverState/stateLabel").GetComponent<UILabel>();
			this.mListBtn = this.mRightAnchor.FindChild("ServerListBtn").GetComponent<UIButton>();
			this.mConfirmBtn = this.mRightAnchor.FindChild("LoginBtn").GetComponent<UIButton>();
			this.mConfirmLabel = this.mConfirmBtn.transform.FindChild("Label").GetComponent<UILabel>();
			this.atlas = this.mSelectedNow_stateBall.atlas;
			UIEventListener.Get(this.mListBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickList);
			UIEventListener.Get(this.mConfirmBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickConfirm);
			UIEventListener.Get(this.mRecommendServerIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickRecommend);
			UIEventListener.Get(this.mLastSelectServerIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLastSelect);
			UIEventListener.Get(this.mGoBackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickGoBackBtn);
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)21002, new MobaMessageFunc(this.OnServerSelect));
			MVC_MessageManager.AddListener_view(MobaMasterCode.GetAllGameServers, new MobaMessageFunc(this.OnReceiveServerList));
			MVC_MessageManager.AddListener_view(MobaMasterCode.SelectGameArea, new MobaMessageFunc(this.OnReceiveGateEnterConfirm));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)21002, new MobaMessageFunc(this.OnServerSelect));
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.GetAllGameServers, new MobaMessageFunc(this.OnReceiveServerList));
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.SelectGameArea, new MobaMessageFunc(this.OnReceiveGateEnterConfirm));
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.clickerCounter = 0;
			if (this.cMgr == null)
			{
				this.cMgr = new CoroutineManager();
			}
			this.cMgr.StartCoroutine(this.RefreshSenderFlag(), true);
			AutoTestController.InvokeTestLogic(AutoTestTag.SelectServer, delegate
			{
				this.OnClickConfirm(null);
			}, 1f);
		}

		public override void HandleBeforeCloseView()
		{
			base.HandleBeforeCloseView();
			if (this.cMgr != null)
			{
				this.cMgr.StopAllCoroutine();
			}
		}

		public override void Destroy()
		{
			this.cMgr = null;
			this.mLeftAnchor = null;
			this.mServerList = null;
			this.mRightAnchor = null;
			this.mRecommendServerIcon = null;
			this.mRecommendServerIcon_tex = null;
			this.mRecommendServerIcon_lab = null;
			this.mLastSelectServerIcon = null;
			this.mLastSelectServerIcon_tex = null;
			this.mLastSelectServerIcon_lab = null;
			this.mRecommendServerBtn = null;
			this.mLastSelectServerBtn = null;
			this.mServerListGrid = null;
			this.mBtnCache = null;
			this.mSelectedNow = null;
			this.mSelectedNow_tex = null;
			this.mSelectedNow_lab = null;
			this.mSelectedNow_stateBall = null;
			this.mSelectedNow_stateLabel = null;
			this.mListBtn = null;
			this.mConfirmBtn = null;
			Resources.UnloadAsset(this.atlas.texture);
			this.atlas = null;
		}

		private void SetLanguage()
		{
			this.mServerList.FindChild("Scroll View/Fix_1/titleLabel").GetComponent<UILabel>().text = this.GetStringById("ChooseServerUI_RecommendServer", "推荐服务器");
			this.mServerList.FindChild("Scroll View/Fix_2/titleLabel").GetComponent<UILabel>().text = this.GetStringById("ChooseServerUI_LastLoginServer", "上次登录服务器");
			this.mServerList.FindChild("Scroll View/titleLabel").GetComponent<UILabel>().text = this.GetStringById("ChooseServerUI_Button_ServerList", "服务器列表");
			this.mRecommendServerIcon.FindChild("titleLabel").GetComponent<UILabel>().text = this.GetStringById("ChooseServerUI_RecommendServer", "推荐服务器");
			this.mLastSelectServerIcon.FindChild("titleLabel").GetComponent<UILabel>().text = this.GetStringById("ChooseServerUI_LastLoginServer", "上次登录服务器");
			this.mRightAnchor.FindChild("Cutlines/group1/stateLabel").GetComponent<UILabel>().text = this.GetStringById("ServerStateUI_Fluent", "流畅");
			this.mRightAnchor.FindChild("Cutlines/group2/stateLabel").GetComponent<UILabel>().text = this.GetStringById("ServerStateUI_Congestion", "拥挤");
			this.mRightAnchor.FindChild("Cutlines/group3/stateLabel").GetComponent<UILabel>().text = this.GetStringById("ServerStateUI_Full", "爆满");
			this.mRightAnchor.FindChild("Cutlines/group4/stateLabel").GetComponent<UILabel>().text = this.GetStringById("ServerStateUI_Maintenance", "维护");
			UIEventListener.Get(this.mRightAnchor.FindChild("Cutlines/group4/stateLabel").gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickNeedLoginByPhone);
			this.mRightAnchor.FindChild("ServerListBtn/Label").GetComponent<UILabel>().text = this.GetStringById("ChooseServerUI_Button_ServerList", "选择服务器");
		}

		private void OnClickNeedLoginByPhone(GameObject go)
		{
			this.fixTime++;
			if (this.fixTime > 20)
			{
				Singleton<LoginView_New>.Instance.isFouceLoginByPhone = true;
			}
		}

		private string GetStringById(string id, string defaultStr)
		{
			return defaultStr;
		}

		private AreaInfo GetRecommend()
		{
			AreaInfo areaInfo = null;
			areaInfo = ModelManager.Instance.Get_RecommendArea();
			if (areaInfo == null)
			{
				using (Dictionary<int, AreaInfo>.Enumerator enumerator = ModelManager.Instance.Get_AreaList_X().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<int, AreaInfo> current = enumerator.Current;
						areaInfo = current.Value;
					}
				}
			}
			return areaInfo;
		}

		private void SetRecommend()
		{
			AreaInfo recommend = this.GetRecommend();
			if (recommend == null)
			{
				ClientLogger.Error("没获取到区服信息，请联系zwy");
				return;
			}
			this.mRecommendServerBtn.serverName = recommend.areaName;
			this.mRecommendServerBtn.SetState((AreaState)recommend.areaState);
			this.mRecommendServerIcon_lab.text = recommend.areaName;
			this.ApplyTexture(this.mRecommendServerIcon_tex, recommend.areaImage);
			this.mSelectedArea = recommend;
		}

		private void SetLastSelect()
		{
			AreaInfo areaInfo = ModelManager.Instance.Get_LastLoginServer_X();
			if (areaInfo == null)
			{
				this.mLastSelectServerBtn.gameObject.SetActive(false);
				this.mLastSelectServerBtn.transform.parent.Find("titleLabel").gameObject.SetActive(false);
				this.mLastSelectServerIcon.gameObject.SetActive(false);
				return;
			}
			this.mLastSelectServerBtn.serverName = areaInfo.areaName;
			this.mLastSelectServerBtn.SetState((AreaState)areaInfo.areaState);
			this.mLastSelectServerIcon_lab.text = areaInfo.areaName;
			this.ApplyTexture(this.mLastSelectServerIcon_tex, areaInfo.areaImage);
			this.mSelectedArea = areaInfo;
		}

		private void SetCurrentSelect()
		{
			AreaInfo areaInfo = ModelManager.Instance.Get_currSelectedArea_X();
			this.mSelectedArea = areaInfo;
			this.mSelectedNow_lab.text = areaInfo.areaName;
			this.ApplyTexture(this.mSelectedNow_tex, areaInfo.areaImage);
			if (this.mServerList.gameObject.activeInHierarchy)
			{
				this.SetAllBtnSelectState(areaInfo.areaName);
			}
			switch (areaInfo.areaState)
			{
			case 0:
				this.mSelectedNow_stateBall.spriteName = "Select_server_round_gray";
				this.mSelectedNow_stateLabel.text = this.GetStringById("ServerStateUI_Maintenance", "维护");
				this.mSelectedNow_stateLabel.color = new Color32(191, 191, 191, 255);
				this.mConfirmBtn.isEnabled = false;
				this.mConfirmLabel.text = this.GetStringById("ServerStateUI_Maintenance", "维护");
				this.mConfirmLabel.gradientTop = new Color32(253, 253, 253, 255);
				this.mConfirmLabel.gradientBottom = new Color32(184, 184, 184, 255);
				this.mConfirmLabel.effectColor = new Color32(55, 55, 55, 255);
				break;
			case 1:
				this.mSelectedNow_stateBall.spriteName = "Select_server_round_green";
				this.mSelectedNow_stateLabel.text = this.GetStringById("ServerStateUI_Fluent", "流畅");
				this.mSelectedNow_stateLabel.color = new Color32(60, 217, 0, 255);
				this.mConfirmBtn.isEnabled = true;
				this.mConfirmLabel.text = this.GetStringById("ServerStateUI_Button_OK", "确定");
				this.mConfirmLabel.gradientTop = new Color32(254, 254, 252, 255);
				this.mConfirmLabel.gradientBottom = new Color32(244, 218, 94, 255);
				this.mConfirmLabel.effectColor = new Color32(111, 24, 6, 255);
				break;
			case 2:
				this.mSelectedNow_stateBall.spriteName = "Select_server_round_orange";
				this.mSelectedNow_stateLabel.text = this.GetStringById("ServerStateUI_Congestion", "拥挤");
				this.mSelectedNow_stateLabel.color = new Color32(244, 117, 0, 255);
				this.mConfirmBtn.isEnabled = true;
				this.mConfirmLabel.text = this.GetStringById("ServerStateUI_Button_OK", "确定");
				this.mConfirmLabel.gradientTop = new Color32(254, 254, 252, 255);
				this.mConfirmLabel.gradientBottom = new Color32(244, 218, 94, 255);
				this.mConfirmLabel.effectColor = new Color32(111, 24, 6, 255);
				break;
			case 3:
				this.mSelectedNow_stateBall.spriteName = "Select_server_round_red";
				this.mSelectedNow_stateLabel.text = this.GetStringById("ServerStateUI_Full", "爆满");
				this.mSelectedNow_stateLabel.color = new Color32(222, 24, 11, 255);
				this.mConfirmBtn.isEnabled = true;
				this.mConfirmLabel.text = this.GetStringById("ServerStateUI_Button_OK", "确定");
				this.mConfirmLabel.gradientTop = new Color32(254, 254, 252, 255);
				this.mConfirmLabel.gradientBottom = new Color32(244, 218, 94, 255);
				this.mConfirmLabel.effectColor = new Color32(111, 24, 6, 255);
				break;
			default:
				this.mSelectedNow_stateBall.spriteName = "Select_server_round_red";
				this.mSelectedNow_stateLabel.text = "未知";
				this.mSelectedNow_stateLabel.color = new Color32(222, 24, 11, 255);
				this.mConfirmBtn.isEnabled = true;
				this.mConfirmLabel.text = "未知状态";
				this.mConfirmLabel.gradientTop = new Color32(254, 254, 252, 255);
				this.mConfirmLabel.gradientBottom = new Color32(244, 218, 94, 255);
				this.mConfirmLabel.effectColor = new Color32(111, 24, 6, 255);
				break;
			}
		}

		private void SetAccInfo()
		{
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				this.mAccNO.transform.parent.gameObject.SetActive(false);
				return;
			}
			string text = ModelManager.Instance.Get_accountData_filed_X("UserName");
			if (!string.IsNullOrEmpty(text))
			{
				int startIndex = (text.Length <= 4) ? 0 : (text.Length - 4);
				this.mAccNO.text = text.Substring(startIndex);
			}
			else
			{
				this.mAccNO.text = "****";
			}
		}

		private void ExitGameCall()
		{
		}

		private void InitBtnObject()
		{
			Dictionary<int, AreaInfo> dictionary = ModelManager.Instance.Get_AreaList_X();
			if (dictionary.Count == 0)
			{
				ClientLogger.Error("没收到服务器数据");
				CtrlManager.ShowMsgBox("错误", "无法获得服务器列表，请稍后再试", new Action(this.ExitGameCall), PopViewType.PopOneButton, "确定", "取消", null);
			}
			this.mBtnGridControllers.Clear();
			foreach (KeyValuePair<int, AreaInfo> current in dictionary)
			{
				GameObject gameObject = NGUITools.AddChild(this.mServerListGrid.gameObject, this.mBtnCache);
				gameObject.SetActive(true);
				AreaButtonCtrl component = gameObject.GetComponent<AreaButtonCtrl>();
				component.BindingAreaInfo(current.Value);
				this.mBtnGridControllers.Add(component);
			}
			this.mServerListGrid.Reposition();
		}

		private void ApplyTexture(UITexture target, string fileName)
		{
			if (fileName == null)
			{
				target.mainTexture = Resources.Load<Texture>("Texture/ServerIcon/Icons_server_cancer");
			}
			else
			{
				target.mainTexture = Resources.Load<Texture>("Texture/ServerIcon/" + fileName);
			}
		}

		private void UploadDefaultSelect()
		{
			if (this.mSelectedArea == null)
			{
				ClientLogger.Error("设备第一次登陆且未获取到区服信息");
				return;
			}
			ModelManager.Instance.Set_curLoginServerIndexByName_X(this.mSelectedArea.areaName);
		}

		private void UpdateCurrLoginServerIndex()
		{
			if (this.mSelectedArea != null)
			{
				ModelManager.Instance.Set_curLoginServerIndexByName_X(this.mSelectedArea.areaName);
			}
		}

		private void OnReceiveGateEnterConfirm(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse.ReturnCode == 0 && this.mServerMsgListenerTask != null && this.cMgr != null)
				{
					this.cMgr.StopCoroutine(this.mServerMsgListenerTask);
					this.mServerMsgListenerTask = null;
				}
			}
		}

		private void OnReceiveServerList(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse.ReturnCode == 0)
				{
					this.refreshBtnStateFlag = true;
					this.mServerListScrollView.alpha = 0.01f;
					for (int i = this.mServerListGrid.transform.childCount - 1; i >= 1; i--)
					{
						UnityEngine.Object.Destroy(this.mServerListGrid.transform.GetChild(i).gameObject);
					}
					AreaInfo areaInfo = this.GetRecommend();
					this.mRecommendServerBtn.SetState((AreaState)areaInfo.areaState);
					areaInfo = ModelManager.Instance.Get_LastLoginServer_X();
					this.mLastSelectServerBtn.SetState((AreaState)areaInfo.areaState);
					this.InitBtnObject();
					ModelManager.Instance.Set_curLoginServerIndexByName_X(this.mSelectedNow_lab.text);
					this.SetAllBtnSelectState(this.mSelectedNow_lab.text);
					this.SetCurrentSelect();
					this.UpdateCurrLoginServerIndex();
				}
			}
		}

		private void OnServerSelect(MobaMessage msg)
		{
			string text = msg.Param.ToString();
			ModelManager.Instance.Set_curLoginServerIndexByName_X(text);
			this.SetAllBtnSelectState(text);
			this.SetCurrentSelect();
		}

		private void SetBtnSelectState(string targetName, AreaButtonCtrl arg)
		{
			arg.SetChosen(arg.serverName == targetName);
		}

		private void SetAllBtnSelectState(string targetName)
		{
			this.SetBtnSelectState(targetName, this.mLastSelectServerBtn);
			this.SetBtnSelectState(targetName, this.mRecommendServerBtn);
			foreach (AreaButtonCtrl current in this.mBtnGridControllers)
			{
				this.SetBtnSelectState(targetName, current);
			}
		}

		[DebuggerHidden]
		private IEnumerator RefreshSenderFlag()
		{
			AreaView.<RefreshSenderFlag>c__Iterator15C <RefreshSenderFlag>c__Iterator15C = new AreaView.<RefreshSenderFlag>c__Iterator15C();
			<RefreshSenderFlag>c__Iterator15C.<>f__this = this;
			return <RefreshSenderFlag>c__Iterator15C;
		}

		private void Clicker()
		{
			this.clickerCounter++;
			if (this.clickerCounter == 10)
			{
				Singleton<TipView>.Instance.ShowViewSetText("Excuse Me???", 1f);
			}
			else if (this.clickerCounter == 20)
			{
				Singleton<TipView>.Instance.ShowViewSetText("出什么问题了吗？[ffb800]【连续点击20次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 40)
			{
				Singleton<TipView>.Instance.ShowViewSetText("游戏进不去光戳我也没用啊！[ffb800]【连续点击40次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 66)
			{
				Singleton<TipView>.Instance.ShowViewSetText("快停下你罪恶的指头！你戳痛我了！[ffb800]【连续点击66次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 90)
			{
				Singleton<TipView>.Instance.ShowViewSetText("你以为这是个点击游戏吗？[ffb800]【连续点击90次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 110)
			{
				Singleton<TipView>.Instance.ShowViewSetText("我跟你讲我要报警了！！[ffb800]【连续点击110次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 200)
			{
				Singleton<TipView>.Instance.ShowViewSetText("你是不是没有女朋友啊？[ffb800]【连续点击200次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 300)
			{
				Singleton<TipView>.Instance.ShowViewSetText("你真的好无聊啊！！[ffb800]【连续点击300次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 500)
			{
				Singleton<TipView>.Instance.ShowViewSetText("我不管你了，你自己玩吧！[ffb800]【连续点击500次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 666)
			{
				Singleton<TipView>.Instance.ShowViewSetText("666666666[ffb800]【连续点击666次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 999)
			{
				Singleton<TipView>.Instance.ShowViewSetText("每个人都有小梦想， 梦想就在前方[ffb800]【连续点击999次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 1000)
			{
				Singleton<TipView>.Instance.ShowViewSetText("恭喜！解锁“[FFE200]金手指[-]”成就！[ffb800]【连续点击1000次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 1001)
			{
				Singleton<TipView>.Instance.ShowViewSetText("一千零一夜， 小梦每一天都在努力【连续点击1001次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 1002)
			{
				Singleton<TipView>.Instance.ShowViewSetText(" 后续还有彩蛋！[ffb800]【连续点击1002次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter > 1000 && this.clickerCounter % 10 == 0)
			{
				Singleton<TipView>.Instance.ShowViewSetText("【连续点击" + this.clickerCounter + "次服务器列表按钮】", 1f);
			}
			else if (this.clickerCounter == 2017)
			{
				Singleton<TipView>.Instance.ShowViewSetText("上海小梦，感谢有您！", 1f);
			}
			else if (this.clickerCounter == 8)
			{
				Singleton<TipView>.Instance.ShowViewSetText("S1赛季冠军航一说：颜值与实力并存。没错我就是航家班的航一！", 1f);
			}
			else if (this.clickerCounter == 28)
			{
				Singleton<TipView>.Instance.ShowViewSetText("咖啡大蒜：蒜家出征 寸草不生！", 1f);
			}
			else if (this.clickerCounter == 18)
			{
				Singleton<TipView>.Instance.ShowViewSetText("小刀：新手福音业界良心，触手TV13669小刀刀是也", 1f);
			}
		}

		private void OnClickList(GameObject obj = null)
		{
			this.Clicker();
			if (this.sendMsgFlag && this.mServerList != null && this.mServerList.gameObject.activeInHierarchy && this.mServerList.GetComponent<UIWidget>().alpha > 0.8f)
			{
				this.sendMsgFlag = false;
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "更新服务器列表...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaMasterCode.GetAllGameServers, param, new object[0]);
			}
			if (this.refreshBtnStateFlag)
			{
				this.mServerListGrid.Reposition();
				this.mServerListScrollView.alpha = 1f;
				this.SetAllBtnSelectState(this.mSelectedNow_lab.text);
				this.refreshBtnStateFlag = false;
			}
		}

		private static int GetSelectedServer()
		{
			if (!Singleton<AreaView>.Instance._autoTest)
			{
				return ModelManager.Instance.Get_currLoginServerIndex_X();
			}
			List<ServerInfo> list = ModelManager.Instance.Get_ServerList_X();
			int num = 0;
			foreach (ServerInfo current in list)
			{
				if (current.servername == "体验服" || current.servername == "AutoTest")
				{
					return num;
				}
				num++;
			}
			return 0;
		}

		[DebuggerHidden]
		private IEnumerator ServerMsgConfirm()
		{
			return new AreaView.<ServerMsgConfirm>c__Iterator15D();
		}

		private void OnClickConfirm(GameObject obj = null)
		{
			ServerInfo serverInfo = ModelManager.Instance.Get_ServerList_X()[AreaView.GetSelectedServer()];
			if (serverInfo.serverState == 0)
			{
				Singleton<TipView>.Instance.ShowViewSetText("抱歉，本地选服数据有误，请重新点击服务器按钮选择服务器。", 1f);
				return;
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在连接服务器...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaMasterCode.SelectGameArea, param, new object[]
			{
				serverInfo.areaId
			});
			if (serverInfo != null)
			{
				ClientData clientData = ModelManager.Instance.Get_ClientData_X();
				clientData.AppVersion = serverInfo.appVer;
				clientData.BindateMD5 = serverInfo.bindataMD5;
				clientData.BindateVer = serverInfo.bindataVer;
				clientData.AppUpgradeUrl = serverInfo.bindataURL;
				this.mServerMsgListenerTask = this.cMgr.StartCoroutine(this.ServerMsgConfirm(), true);
			}
			else
			{
				ClientLogger.Error("服务器信息错误");
				CtrlManager.ShowMsgBox("服务器信息错误", "选择服务器在服务器列表中找不到", delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
			}
		}

		private void OnClickRecommend(GameObject obj = null)
		{
			string text = this.mRecommendServerIcon_lab.text;
			if (this.mRecommendServerIcon_lab.text == this.mSelectedNow_lab.text)
			{
				return;
			}
			ModelManager.Instance.Set_curLoginServerIndexByName_X(text);
			this.SetCurrentSelect();
			this.refreshBtnStateFlag = true;
		}

		private void OnClickLastSelect(GameObject obj = null)
		{
			string text = this.mLastSelectServerIcon_lab.text;
			if (this.mLastSelectServerIcon_lab.text == this.mSelectedNow_lab.text)
			{
				return;
			}
			ModelManager.Instance.Set_curLoginServerIndexByName_X(text);
			this.SetCurrentSelect();
			this.refreshBtnStateFlag = true;
		}

		private void OnClickGoBackBtn(GameObject obj = null)
		{
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				InitSDK.instance.SDKLogout(false);
			}
			else if (GlobalSettings.isLoginByAnySDK)
			{
				InitSDK.instance.SDKAnySDKLogout(false);
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.AreaViewNew_goback, null, false);
			}
		}
	}
}
