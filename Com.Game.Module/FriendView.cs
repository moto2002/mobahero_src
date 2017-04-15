using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros.Pvp;
using MobaHeros.Pvp.State;
using MobaProtocol;
using MobaProtocol.Data;
using MobaServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace Com.Game.Module
{
	public class FriendView : BaseView<FriendView>
	{
		public enum State
		{
			HaveMessage,
			Online,
			OffLine
		}

		private Transform EmptyShow;

		private Transform LeftAnchor;

		private Transform FindPanel;

		private Transform FindPanelBg;

		private UISprite FindPanelInputBg;

		private Transform AddFirend;

		private Transform AddButton;

		private Transform FindButton;

		private UIInput findInput;

		private Transform FriendPanel;

		private GameObject selectObj;

		private FriendItem selectFriendItem;

		private Transform UserInfo;

		private UILabel UserInfoName;

		private UILabel UserInfoState;

		private UILabel UserInfoRankValue;

		private UILabel UserInfoWinValue;

		private Transform TalkInfo;

		private Transform OffLineWarn;

		private UILabel FriendTxt;

		private Transform SendCardBtn;

		private Transform MoveBlackBtn;

		private Transform DelectFriend;

		private UISprite seletBg;

		private Transform FindFriendItem;

		private Transform SendMessageBtn;

		private UIInput chatInput;

		private ChatInput _chatInput;

		private Transform ChatContent;

		private UIPanel ChatContentPanel;

		private UIScrollView ChatContentScrollView;

		private SendChatMessage sendChatmessage;

		private EasyCodeScannerControl QRControl;

		private Transform QRScanner;

		private Transform QRPlane;

		private Transform MyQR;

		private Transform Net;

		private Transform CancelBtn;

		private Transform BackBtn;

		private Transform DeleteBtn;

		private UILabel MyQRName;

		private UITexture myQRTexture;

		private Transform ShareBtn;

		private Transform SaveBtn;

		private UILabel myId;

		private Transform MyQRPanel;

		private Transform MyQRPanelBackBtn;

		private UITexture headTex;

		private Transform QRScannerPanel;

		private Transform QRScannerPhotoBtn;

		private Transform QRScannerMyQRBtn;

		private Transform QRScannerPanelBackBtn;

		private Transform QRScannerPanelMyQRButton;

		private UILabel BottleLv;

		private UILabel RankLv;

		private UILabel MeiliLv;

		private Transform RadarPanel;

		private Transform BackFromRadar;

		private UITexture MyPortrait;

		private Transform FriendsList;

		private GameObject RadarFriendItem;

		private Transform RadarFriendReq;

		private UISprite RadarFriendStatu;

		private UILabel RadarFriendStatuText;

		private Transform AddRadarBackBtn;

		private long findSelectId;

		public List<long> newMessageList = new List<long>();

		private List<FriendItem> friendItemList = new List<FriendItem>();

		private bool needShowFindFriendItem;

		private GameObject firendChatTextPre;

		private CoroutineManager coroutineManager = new CoroutineManager();

		private Task FixScrollBarTask;

		private AllochroicLabelChecker alc = new AllochroicLabelChecker();

		private Task listTask;

		private bool isRadarAdd;

		private bool openViewOnce = true;

		private string applyChoice;

		public string selectid;

		public GameObject TempObj;

		private int ChatContentHight;

		private TweenAlpha m_AlphaController;

		private GameObject friendItemPre;

		private int Idx;

		private Texture2D QRTexture;

		public FriendView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/FriendView");
			this.WindowTitle = LanguageManager.Instance.GetStringById("FriendsUI_MyFriend");
		}

		public override void Init()
		{
			base.Init();
			this.sendChatmessage = this.gameObject.GetComponent<SendChatMessage>();
			this.LeftAnchor = this.transform.Find("LeftAnchor");
			this.EmptyShow = this.LeftAnchor.Find("NoFriend");
			this.FindPanel = this.transform.Find("Anchor/FindPanel");
			this.FindPanelBg = this.transform.Find("Anchor/FindPanel/Bg");
			this.AddFirend = this.transform.Find("LeftAnchor/BottomBg/AddFirend");
			this.AddButton = this.transform.Find("Anchor/FindPanel/AddButton");
			this.FindButton = this.transform.Find("Anchor/FindPanel/FindBtn");
			this.findInput = this.transform.Find("Anchor/FindPanel/Input").GetComponent<UIInput>();
			this.FriendPanel = this.transform.Find("LeftAnchor/FirendPanel");
			this.UserInfo = this.transform.Find("RightAnchor/Panel/UserInfo");
			this.TalkInfo = this.transform.Find("RightAnchor/Panel/TalkInfo");
			this.SendCardBtn = this.transform.Find("RightAnchor/Panel/UserInfo/SendCard");
			this.MoveBlackBtn = this.transform.Find("RightAnchor/Panel/UserInfo/MoveBlack");
			this.DelectFriend = this.transform.Find("RightAnchor/Panel/UserInfo/DelectFriend");
			this.FindFriendItem = this.transform.Find("Anchor/FindPanel/FindFriendItem");
			this.UserInfoName = this.UserInfo.Find("Name").GetComponent<UILabel>();
			this.UserInfoState = this.UserInfo.Find("State").GetComponent<UILabel>();
			this.UserInfoRankValue = this.UserInfo.Find("RankValue").GetComponent<UILabel>();
			this.UserInfoWinValue = this.UserInfo.Find("WinValue").GetComponent<UILabel>();
			this.OffLineWarn = this.TalkInfo.Find("OffLineWarn");
			this.FriendTxt = this.TalkInfo.Find("Chat Window/ChatText").GetComponent<UILabel>();
			this.SendMessageBtn = this.TalkInfo.Find("SendMessageBtn");
			this.chatInput = this.TalkInfo.Find("Chat Window/Chat Input").GetComponent<UIInput>();
			EventDelegate item = new EventDelegate(new EventDelegate.Callback(this.ChangeLabelText));
			this.chatInput.onChange = new List<EventDelegate>();
			this.chatInput.onChange.Add(item);
			this._chatInput = this.TalkInfo.Find("Chat Window/Chat Input").GetComponent<ChatInput>();
			this.ChatContent = this.TalkInfo.Find("Chat Window/ChatContent");
			this.ChatContentScrollView = this.ChatContent.GetComponent<UIScrollView>();
			this.ChatContentPanel = this.ChatContent.GetComponent<UIPanel>();
			this.firendChatTextPre = Resources.Load<GameObject>("Prefab/UI/Home/FriendChatText");
			EventDelegate item2 = new EventDelegate(this.sendChatmessage, "SearchOnSubmit");
			this.FindPanelInputBg = this.transform.Find("Anchor/FindPanel/InputBg").GetComponent<UISprite>();
			this.findInput.onSubmit.Add(item2);
			this.QRScanner = this.transform.Find("Anchor/FindPanel/QRScanner");
			this.QRPlane = this.transform.Find("Anchor/FindPanel/QRPlane");
			this.QRControl = this.transform.Find("Anchor/FindPanel/QRPlane").GetComponent<EasyCodeScannerControl>();
			this.MyQR = this.transform.Find("Anchor/FindPanel/MyQR");
			this.Net = this.transform.Find("Anchor/FindPanel/Net");
			this.CancelBtn = this.FindFriendItem.Find("CancelBtn");
			this.BackBtn = this.transform.Find("Anchor/FindPanel/BackBtn");
			this.DeleteBtn = this.transform.Find("Anchor/FindPanel/Delet");
			this.myId = this.transform.Find("Anchor/FindPanel/TitleLabel").GetComponent<UILabel>();
			this.MyQRName = this.transform.Find("Anchor/FindPanel/MyQRPanel/NameLabel").GetComponent<UILabel>();
			this.myQRTexture = this.transform.Find("Anchor/FindPanel/MyQRPanel/Pic").GetComponent<UITexture>();
			this.ShareBtn = this.transform.Find("Anchor/FindPanel/MyQRPanel/ShareButton");
			this.SaveBtn = this.transform.Find("Anchor/FindPanel/MyQRPanel/SaveButton");
			this.MyQRPanel = this.transform.Find("Anchor/FindPanel/MyQRPanel");
			this.MyQRPanelBackBtn = this.transform.Find("Anchor/FindPanel/MyQRPanel/BackBtn");
			this.MyQRPanel.gameObject.SetActive(false);
			this.headTex = this.transform.Find("Anchor/FindPanel/MyQRPanel/HeadPic").GetComponent<UITexture>();
			this.QRScannerPanel = this.transform.Find("Anchor/FindPanel/QRScannerPanel");
			this.QRScannerPanelBackBtn = this.transform.Find("Anchor/FindPanel/QRScannerPanel/BackBtn");
			this.QRScannerPanelMyQRButton = this.transform.Find("Anchor/FindPanel/QRScannerPanel/MyQRButton");
			this.BottleLv = this.FindFriendItem.Find("Moping/Label").GetComponent<UILabel>();
			this.RankLv = this.FindFriendItem.Find("Paiwei/Label").GetComponent<UILabel>();
			this.MeiliLv = this.FindFriendItem.Find("Meili/Label").GetComponent<UILabel>();
			this.RadarPanel = this.transform.Find("Anchor/FindPanel/Radar");
			this.BackFromRadar = this.transform.Find("Anchor/FindPanel/Radar/BackBtn");
			this.MyPortrait = this.transform.Find("Anchor/FindPanel/Radar/Portrait").GetComponent<UITexture>();
			this.FriendsList = this.transform.Find("Anchor/FindPanel/Radar/Friends");
			this.RadarFriendItem = Resources.Load<GameObject>("Prefab/UI/Home/RadarFriendItem");
			this.RadarFriendReq = this.transform.Find("Anchor/FindPanel/AddFriendPanel");
			this.RadarFriendStatu = this.RadarFriendReq.transform.Find("Status").GetComponent<UISprite>();
			this.RadarFriendStatuText = this.RadarFriendReq.transform.Find("Status/Label").GetComponent<UILabel>();
			this.AddRadarBackBtn = this.RadarFriendReq.Find("Label");
			UIEventListener.Get(this.AddFirend.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnAddFirend);
			UIEventListener.Get(this.BackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnFindPanelBg);
			UIEventListener.Get(this.AddButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnAddButton);
			UIEventListener.Get(this.FindButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnFindButton);
			UIEventListener.Get(this.DelectFriend.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnDelectFirend);
			UIEventListener.Get(this.MoveBlackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnMoveBlack);
			UIEventListener.Get(this.SendMessageBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSendMessageBtn);
			UIEventListener.Get(this.QRScanner.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnQRScanner);
			UIEventListener.Get(this.CancelBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnCancelBtn);
			UIEventListener.Get(this.DeleteBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnDeleteBtn);
			UIEventListener.Get(this.MyQR.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnMyQR);
			UIEventListener.Get(this.MyQRPanelBackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnMyQRPanelBackBtn);
			UIEventListener.Get(this.QRScannerPanelBackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnQRScannerPanelBackBtn);
			UIEventListener.Get(this.QRScannerPanelMyQRButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnMyQR);
			UIEventListener.Get(this.Net.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnRadarBtn);
			UIEventListener.Get(this.BackFromRadar.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnRadarBtn);
			UIEventListener.Get(this.AddRadarBackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnCancelAddRadarBtn);
		}

		public override void HandleAfterOpenView()
		{
			this.FindPanel.gameObject.SetActive(false);
			this.UserInfo.gameObject.SetActive(false);
			this.TalkInfo.gameObject.SetActive(false);
			this.RefreshUI();
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			this.gameObject.GetComponent<UIPanel>().depth = 10;
			Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
			MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_OnFriendList));
			MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_ApplyAddFriend, new MobaMessageFunc(this.OnGetMsg__ApplyAddFriend));
			MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg__ModifyFriendStatus));
			MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_RaderFindFriend, new MobaMessageFunc(this.OnGetMsg__GetRadarFriend));
			MVC_MessageManager.AddListener_view(MobaGameCode.GetUserInfoBySummId, new MobaMessageFunc(this.OnGetMsg_GetUserInfoBySummId));
			MobaMessageManager.RegistMessage(MobaChatCode.Chat_ListenPrivate, new MobaMessageFunc(this.OnGetMsg_Chat_ListenPrivate));
			MobaMessageManager.RegistMessage(MobaChatCode.Chat_PullHistory, new MobaMessageFunc(this.OnGetMsg_Chat_PullHistory));
			MobaMessageManager.RegistMessage((ClientMsg)21069, new MobaMessageFunc(this.OnGetMsg_chatviewSendMessage));
			MobaMessageManager.RegistMessage((ClientMsg)23063, new MobaMessageFunc(this.OnGetMsg_ReceiveFriendChatMessage));
			this.myId.text = ModelManager.Instance.Get_userData_X().SummonerId.ToString();
			SendMsgManager.SendMsgParam sendMsgParam = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Tips_GettingGoodFriendInformation"), true, 15f);
			this.openViewOnce = true;
		}

		private void OnGetMsg_GetUserInfoBySummId(MobaMessage msg)
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
				this.GetUserInfoBySummId(num, operationResponse.DebugMessage, null);
			}
			else
			{
				FriendData friendUserInfo = ModelManager.Instance.GetFriendUserInfo();
				this.GetUserInfoBySummId(num, operationResponse.DebugMessage, friendUserInfo);
			}
		}

		private void GetUserInfoBySummId(int arg1, string arg2, FriendData arg3)
		{
			if (!Singleton<FriendView>.Instance.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.FindPanel.gameObject.activeInHierarchy && this.needShowFindFriendItem)
			{
				this.needShowFindFriendItem = false;
				if (arg3 == null)
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_NoThisSummonerID"), 1f);
					return;
				}
				if (arg3.TargetId == 0L && arg3.PictureFrame == 0)
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_NoThisSummonerID"), 1f);
					return;
				}
				this.AddButton.Find("Sprite").GetComponent<UISprite>().spriteName = "Add_friend_icons_friend_01";
				this.AddButton.Find("Label").GetComponent<UILabel>().color = new Color32(0, 246, 229, 255);
				this.AddButton.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_AddFriends");
				this.FindFriendItem.gameObject.SetActive(true);
				this.AddButton.gameObject.SetActive(true);
				FriendItem component = this.FindFriendItem.GetComponent<FriendItem>();
				component.name.text = arg3.TargetName;
				this.QRScanner.gameObject.SetActive(false);
				this.MyQR.gameObject.SetActive(false);
				this.Net.gameObject.SetActive(false);
				this.AddButton.gameObject.SetActive(true);
				if (this.findInput.value != null)
				{
					this.findSelectId = long.Parse(this.findInput.value);
				}
				component.transform.Find("ID/IDLabel").GetComponent<UILabel>().text = this.findSelectId.ToString();
				SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(arg3.PictureFrame.ToString());
				if (dataById == null)
				{
					component.pictureFrame.spriteName = "pictureframe_0000";
				}
				else
				{
					component.pictureFrame.spriteName = dataById.pictureframe_icon;
				}
				SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(arg3.Icon.ToString());
				if (dataById2 == null)
				{
					component.texture.spriteName = "headportrait_0001";
				}
				else
				{
					component.texture.spriteName = dataById2.headportrait_icon;
				}
				component.level.text = CharacterDataMgr.instance.GetUserLevel(arg3.Exp).ToString();
				this.FindPanelInputBg.height = 623;
				this.BottleLv.text = arg3.bottlelevel.ToString();
				this.RankLv.text = LanguageManager.Instance.GetStringById(this.GetState(arg3.LadderScore));
				this.MeiliLv.text = arg3.charm.ToString();
			}
			else
			{
				FriendItem friendItem = this.friendItemList.Find((FriendItem obj) => obj.Name == arg3.TargetName);
				if (friendItem == null)
				{
					return;
				}
				friendItem.SetGameState((GameStatus)arg3.Status);
				friendItem.level.text = CharacterDataMgr.instance.GetUserLevel(arg3.Exp).ToString();
				SysSummonersPictureframeVo dataById3 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(arg3.PictureFrame.ToString());
				friendItem.pictureFrame.spriteName = dataById3.pictureframe_icon;
				SysSummonersHeadportraitVo dataById4 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(arg3.Icon.ToString());
				friendItem.texture.spriteName = dataById4.headportrait_icon;
				this.FindPanelInputBg.height = 623;
			}
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_OnFriendList));
			MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_ApplyAddFriend, new MobaMessageFunc(this.OnGetMsg__ApplyAddFriend));
			MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg__ModifyFriendStatus));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetUserInfoBySummId, new MobaMessageFunc(this.OnGetMsg_GetUserInfoBySummId));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetFriendMessages, new MobaMessageFunc(this.OnGetMsg__GetFriendMessages));
			MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_RaderFindFriend, new MobaMessageFunc(this.OnGetMsg__GetRadarFriend));
			MobaMessageManager.UnRegistMessage(MobaChatCode.Chat_ListenPrivate, new MobaMessageFunc(this.OnGetMsg_Chat_ListenPrivate));
			MobaMessageManager.UnRegistMessage(MobaChatCode.Chat_PullHistory, new MobaMessageFunc(this.OnGetMsg_Chat_PullHistory));
			MobaMessageManager.UnRegistMessage((ClientMsg)21069, new MobaMessageFunc(this.OnGetMsg_chatviewSendMessage));
			MobaMessageManager.UnRegistMessage((ClientMsg)23063, new MobaMessageFunc(this.OnGetMsg_ReceiveFriendChatMessage));
			Singleton<MenuView>.Instance.UpdateFriendNew();
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(102, null);
			NetWorkHelper.Instance.client.SendSessionChannelMessage(7, MobaChannel.Chat, dictionary);
			this.openViewOnce = false;
			this.selectObj = null;
			if (this.selectFriendItem != null)
			{
				this.selectFriendItem.select.gameObject.SetActive(false);
			}
			this.selectid = null;
			this.TempObj = null;
		}

		private void OnGetMsg__GetRadarFriend(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.UpdateRadarFriends();
			}
		}

		private void OnGetMsg__GetFriendMessages(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.GetFriendsMessages(num, operationResponse.DebugMessage, null);
			}
			else
			{
				this.GetFriendsMessages(num, operationResponse.DebugMessage, ModelManager.Instance.GetNotificationDataList());
			}
		}

		private void GetFriendsMessages(int arg1, string arg2, List<NotificationData> arg3)
		{
		}

		private void OnGetMsg__ModifyFriendStatus(MobaMessage msg)
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
			if (mobaErrorCode != MobaErrorCode.MyFriendCountLimit)
			{
				if (mobaErrorCode != MobaErrorCode.TargetFriendCountLimit)
				{
					if (mobaErrorCode != MobaErrorCode.Ok)
					{
						this.ModifyFriendStatus(num, operationResponse.DebugMessage, 0, 0L);
					}
					else
					{
						byte b = (byte)operationResponse.Parameters[10];
						long targetSummid = (long)operationResponse.Parameters[102];
						ModelManager.Instance.Get_applyList_X().Remove(ModelManager.Instance.Get_applyList_X().Find((FriendData obj) => obj.TargetId == targetSummid));
						if (b != 1)
						{
							ModelManager.Instance.Get_FriendDataList_X().Remove(ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId == targetSummid));
						}
						this.ModifyFriendStatus(num, operationResponse.DebugMessage, b, targetSummid);
						Singleton<MenuView>.Instance.UpdateFriendNew();
					}
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_FriendsListIsFull"), 1f);
					ModelManager.Instance.Get_applyList_X().Remove(ModelManager.Instance.Get_applyList_X().Find((FriendData obj) => obj.TargetId.ToString() == <OnGetMsg__ModifyFriendStatus>c__AnonStorey24B.<>f__this.applyChoice));
					this.StartGetFriendList();
				}
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_FriendsListIsFull"), 1f);
				ModelManager.Instance.Get_applyList_X().Remove(ModelManager.Instance.Get_applyList_X().Find((FriendData obj) => obj.TargetId.ToString() == <OnGetMsg__ModifyFriendStatus>c__AnonStorey24B.<>f__this.applyChoice));
				this.StartGetFriendList();
			}
		}

		private void ModifyFriendStatus(int arg1, string arg2, byte arg3, long arg4)
		{
			if ((arg3 == 4 || arg3 == 5) && this.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(arg4)))
			{
				this.newMessageList.Remove(ToolsFacade.Instance.GetUserIdBySummId(arg4));
			}
			if (arg3 != 1)
			{
				UnityEngine.Object.DestroyImmediate(this.selectObj);
				this.selectObj = null;
				this.FriendPanel.GetComponent<UIGrid>().Reposition();
				if (arg3 != 2)
				{
					this.CheckselectObj(null);
				}
				Singleton<MenuBottomBarView>.Instance.UpdateFriendNum();
			}
			else
			{
				this.UpdateNewApply(null);
			}
		}

		private void SetUserInfo(FriendItem friendItem)
		{
			this.UserInfoName.text = friendItem.Name;
			this.UserInfoName.gameObject.GetComponent<AllochroicLabelChecker>().FriendRenderLabel(friendItem.lastCharmRank);
			this.UserInfoState.text = friendItem.State;
			this.UserInfoRankValue.text = friendItem.RankValue.ToString();
			this.UserInfoWinValue.text = friendItem.WinValue.ToString();
		}

		private void OnGetMsg__ApplyAddFriend(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			switch (mobaErrorCode)
			{
			case MobaErrorCode.FriendExist:
				Singleton<TipView>.Instance.ShowViewSetText("你们已经是好友啦", 1f);
				return;
			case MobaErrorCode.FriendNotExist:
			case MobaErrorCode.FriendOffline:
			case MobaErrorCode.MyBlackCountLimit:
				IL_50:
				if (mobaErrorCode != MobaErrorCode.Ok)
				{
					this.ApplyAddFriend(num, operationResponse.DebugMessage, null);
					return;
				}
				this.ApplyAddFriend(num, operationResponse.DebugMessage, null);
				return;
			case MobaErrorCode.FriendInBlacklist:
				Singleton<TipView>.Instance.ShowViewSetText("对方在您黑名单中", 1f);
				return;
			case MobaErrorCode.MyFriendCountLimit:
				Singleton<TipView>.Instance.ShowViewSetText("您的好友已达上限", 1f);
				return;
			case MobaErrorCode.TargetFriendCountLimit:
				Singleton<TipView>.Instance.ShowViewSetText("对方的好友已达上限", 1f);
				return;
			case MobaErrorCode.InTargetBlacklist:
				Singleton<TipView>.Instance.ShowViewSetText("您在对方黑名单中", 1f);
				return;
			case MobaErrorCode.ApplyFriendAlready:
				Singleton<TipView>.Instance.ShowViewSetText("已发送好友请求", 1f);
				return;
			}
			goto IL_50;
		}

		private void ApplyAddFriend(int arg1, string arg2, FriendData arg3)
		{
			if (this.isRadarAdd)
			{
				return;
			}
			List<FriendData> list = ModelManager.Instance.Get_blackList_X();
			if (arg1 == 20104)
			{
				if (list.Find((FriendData obj) => obj.TargetId == this.findSelectId) != null)
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_RemoveFromBlackList"), 1f);
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_SorryYouAreBacklisted"), 1f);
				}
				this.AddButton.Find("Sprite").GetComponent<UISprite>().spriteName = "Add_friend_icons_friend_01";
				this.AddButton.Find("Label").GetComponent<UILabel>().color = new Color32(0, 246, 229, 255);
				this.AddButton.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_AddFriends");
				UIEventListener.Get(this.AddButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnAddButton);
			}
		}

		public override void RefreshUI()
		{
			this.StartGetFriendList();
		}

		private void ClearFriendPanel(bool isRightNow = false)
		{
			int childCount = this.FriendPanel.childCount;
			for (int i = 0; i < childCount; i++)
			{
				this.FriendPanel.GetChild(i).gameObject.SetActive(false);
			}
		}

		private void OnGetMsg_OnFriendList(MobaMessage msg)
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
				this.GetFriendList(null);
			}
			else
			{
				this.GetFriendList(null);
			}
		}

		private void GetFriendList(List<FriendData> _friendDataList)
		{
			if (this.gameObject != null && this.gameObject.activeInHierarchy)
			{
				this.StartGetFriendList();
			}
			else
			{
				Singleton<InvitationView>.Instance.AddFriendApplyList(ModelManager.Instance.Get_applyList_X());
				MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_OnFriendList));
			}
		}

		private void StartGetFriendList()
		{
			this.friendItemList.Clear();
			if (this.friendItemPre == null)
			{
				this.friendItemPre = (Resources.Load("Prefab/UI/Home/FriendItem") as GameObject);
			}
			List<FriendData> list = ModelManager.Instance.Get_applyList_X();
			ModelManager.Instance.Get_applyList_X().Sort((FriendData x, FriendData y) => x.TargetId.CompareTo(y.TargetId));
			this.CreateList(this.friendItemPre, list, 0);
			List<FriendData> friendDataList = ModelManager.Instance.Get_FriendDataList_X();
			friendDataList = this.FixFriendDataListByOnline(friendDataList);
			this.CreateList(this.friendItemPre, friendDataList, list.Count);
			int num = ModelManager.Instance.Get_applyList_X().Count + ModelManager.Instance.Get_FriendDataList_X().Count;
			if (num == 0)
			{
				this.EmptyShow.gameObject.SetActive(true);
			}
			else
			{
				this.EmptyShow.gameObject.SetActive(false);
			}
			if (this.FriendPanel.childCount > num)
			{
				for (int i = 0; i < this.FriendPanel.childCount - num; i++)
				{
					this.FriendPanel.GetChild(num + i).gameObject.SetActive(false);
				}
			}
			for (int j = 0; j < this.friendItemList.Count; j++)
			{
				this.friendItemList[j].transform.localPosition = new Vector3(0f, (float)(-176 * j), 0f);
			}
			Singleton<MenuBottomBarView>.Instance.UpdateFriendNum();
		}

		public List<FriendData> FixFriendDataListByOnline(List<FriendData> FriendDataList)
		{
			if (FriendDataList != null && FriendDataList.Count > 0)
			{
				FriendDataList.Sort(delegate(FriendData left, FriendData right)
				{
					if (this.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(left.TargetId)) && !this.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(right.TargetId)))
					{
						return -1;
					}
					if (!this.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(left.TargetId)) && this.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(right.TargetId)))
					{
						return 1;
					}
					if ((int)left.GameStatus != 0 && (int)right.GameStatus != 0)
					{
						if ((int)right.GameStatus == 1 && (int)left.GameStatus != 1 && (int)left.GameStatus != 0)
						{
							return 1;
						}
						if ((int)left.GameStatus != (int)right.GameStatus)
						{
							return -1;
						}
						if (left.TargetId < right.TargetId)
						{
							return -1;
						}
						return 1;
					}
					else
					{
						if ((int)left.GameStatus == 0 && (int)right.GameStatus != 0)
						{
							return 1;
						}
						if ((int)left.GameStatus != (int)right.GameStatus)
						{
							return -1;
						}
						if (left.TargetId < right.TargetId)
						{
							return -1;
						}
						return 1;
					}
				});
			}
			return FriendDataList;
		}

		private void CreateList(GameObject friendItemPre, List<FriendData> FriendDataList = null, int index = 0)
		{
			if (FriendDataList != null && FriendDataList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < FriendDataList.Count; i++)
			{
				GameObject gameObject;
				if (i + index < this.FriendPanel.childCount)
				{
					gameObject = this.FriendPanel.GetChild(i + index).gameObject;
					gameObject.gameObject.SetActive(true);
				}
				else
				{
					gameObject = NGUITools.AddChild(this.FriendPanel.gameObject, friendItemPre);
				}
				FriendItem component = gameObject.GetComponent<FriendItem>();
				this.friendItemList.Add(component);
				component.data = FriendDataList[i];
				component.apply.gameObject.name = component.data.TargetId.ToString();
				component.refuse.gameObject.name = component.data.TargetId.ToString();
				gameObject.name = component.data.TargetId.ToString();
				SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(component.data.PictureFrame.ToString());
				if (dataById == null)
				{
					component.pictureFrame.spriteName = "pictureframe_0000";
				}
				else
				{
					component.pictureFrame.spriteName = dataById.pictureframe_icon;
				}
				SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(component.data.Icon.ToString());
				if (dataById2 == null)
				{
					component.texture.spriteName = "headportrait_0001";
				}
				else
				{
					component.texture.spriteName = dataById2.headportrait_icon;
				}
				component.gameState = (int)component.data.GameStatus;
				if (this.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(component.data.TargetId)))
				{
					component.ShowMessageSign();
				}
				else
				{
					component.HideMessageSign();
				}
				if ((int)component.data.Status == 3)
				{
					component.applyStatus();
					component.friendType = 3;
					if (UIEventListener.Get(component.apply.gameObject).onClick == null)
					{
						UIEventListener.Get(component.apply.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnApplyFriend);
					}
					if (UIEventListener.Get(component.refuse.gameObject).onClick == null)
					{
						UIEventListener.Get(component.refuse.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnRefuseFriend);
					}
				}
				else if ((int)component.data.Status == 1)
				{
					component.friendStatus();
					component.friendType = 1;
					component.SetGameState((GameStatus)component.gameState);
					if (UIEventListener.Get(component.btnObserve.gameObject).onClick == null)
					{
						UIEventListener.Get(component.btnObserve.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnObserveFriend);
					}
				}
				component.RankValue = component.data.LadderScore;
				component.WinValue = component.data.MathWinNum;
				component.State = LanguageManager.Instance.GetStringById(this.GetState(component.data.LadderScore));
				component.Name = component.data.TargetName;
				component.name.text = component.data.TargetName;
				component.lastCharmRank = component.data.CharmRankValue;
				component.id = component.data.TargetId;
				component.gameState = (int)component.data.GameStatus;
				component.level.text = CharacterDataMgr.instance.GetUserLevel(component.data.Exp).ToString();
				component.messages = component.data.Messages;
				this.alc = gameObject.transform.Find("Name").GetComponent<AllochroicLabelChecker>();
				if (null == this.alc)
				{
					gameObject.transform.Find("Name").gameObject.AddComponent<AllochroicLabelChecker>();
				}
				else
				{
					gameObject.transform.Find("Name").GetComponent<AllochroicLabelChecker>().RenderLabel(component.data.CharmRankValue);
				}
				if (this.selectid == component.id.ToString())
				{
					component.select.gameObject.SetActive(true);
					this.TempObj = component.gameObject;
				}
				else
				{
					component.select.gameObject.SetActive(false);
				}
			}
		}

		public string GetState(int score)
		{
			string empty = string.Empty;
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysRankStageVo>();
			int num = 0;
			for (int i = 1; i <= dicByType.Keys.Count; i++)
			{
				if (score >= (dicByType[i.ToString()] as SysRankStageVo).StageScore)
				{
					num = i;
				}
			}
			if (num == 0)
			{
				num = 1;
			}
			return (dicByType[num.ToString()] as SysRankStageVo).StageName;
		}

		private void OnRefuseFriend(GameObject go)
		{
			this.applyChoice = go.transform.parent.name;
			this.selectObj = go.transform.parent.gameObject;
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Tips_Communicating"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ModifyFriendStatus, param, new object[]
			{
				long.Parse(go.name),
				2
			});
		}

		private void OnObserveFriend(GameObject go)
		{
			if (PvpMatchMgr.State != PvpMatchState.None)
			{
				CtrlManager.ShowMsgBox("错误", LanguageManager.Instance.GetStringById("GangUpUI_SpectatorLimit"), delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			PvpStateManager.Instance.ChangeState(new PvpStateObserverLogin(PvpStateCode.PvpObserverLogin, ToolsFacade.Instance.GetUserIdBySummId(long.Parse(go.transform.parent.name)).ToString()));
		}

		private void OnApplyFriend(GameObject go)
		{
			this.applyChoice = go.transform.parent.name;
			this.selectObj = go.transform.parent.gameObject;
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Tips_Communicating"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ModifyFriendStatus, param, new object[]
			{
				long.Parse(go.name),
				1
			});
		}

		private void OnMoveBlack(GameObject go)
		{
			CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("FriendsUI_DialogBox_Title_AddBlackList"), LanguageManager.Instance.GetStringById("FriendsUI_DialogBox_Content_AddBlackList"), new Action<bool>(this.BoolMoveBlack), PopViewType.PopTwoButton, "确定", "取消", null);
		}

		private void BoolMoveBlack(bool bIsTrigger)
		{
			if (bIsTrigger)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Tips_AddBlackList"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ModifyFriendStatus, param, new object[]
				{
					this.selectFriendItem.id,
					5
				});
			}
		}

		private void OnDelectFirend(GameObject go)
		{
			if (this.selectFriendItem != null)
			{
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("FriendsUI_Button_DeleteFriend"), LanguageManager.Instance.GetStringById("FriendsUI_DialogBox_Content_DeleteFriend").Replace("*", this.selectFriendItem.Name), new Action<bool>(this.BoolDelectFirend), PopViewType.PopTwoButton, "确定", "取消", null);
			}
		}

		private void BoolDelectFirend(bool bIsTrigger)
		{
			if (bIsTrigger && this.selectFriendItem != null)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Tips_DeleteFriend"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ModifyFriendStatus, param, new object[]
				{
					this.selectFriendItem.id,
					4
				});
			}
		}

		private void OnGetMsg_ReceiveFriendChatMessage(MobaMessage msg)
		{
			if (msg != null)
			{
				ChatMessageNew chatMessageNew = (ChatMessageNew)msg.Param;
				string userId = ModelManager.Instance.Get_userData_X().UserId;
				if (chatMessageNew.Client.UserId.ToString() == ModelManager.Instance.Get_userData_X().UserId)
				{
					this.AddFriendTxt(chatMessageNew.Client.UserId.ToString(), chatMessageNew.Message, "me", 0);
				}
				else
				{
					this.AddFriendTxt(chatMessageNew.Client.UserId.ToString(), chatMessageNew.Message, chatMessageNew.TimeTick.ToString(), 1);
				}
				ModelManager.Instance.SaveIn_Friend_Chat_MessageListX(ToolsFacade.Instance.GetUserIdBySummId(long.Parse(this.selectid)).ToString(), chatMessageNew);
			}
		}

		private void OnGetMsg_Chat_ListenPrivate(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					string value = (string)operationResponse.Parameters[102];
					Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
					dictionary.Add(102, value);
					NetWorkHelper.Instance.client.SendSessionChannelMessage(4, MobaChannel.Chat, dictionary);
				}
			}
		}

		private void OnGetMsg_Chat_PullHistory(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					byte[] buffer = (byte[])operationResponse.Parameters[174];
					List<byte[]> list = SerializeHelper.Deserialize<List<byte[]>>(buffer);
					for (int i = 0; i < list.Count; i++)
					{
						ChatMessageNew chatMessageNew = SerializeHelper.Deserialize<ChatMessageNew>(list[i]);
						ModelManager.Instance.SaveIn_Friend_Chat_MessageListX(chatMessageNew.Client.UserId.ToString(), chatMessageNew);
					}
				}
				this.UpdateFriendTxt(this.selectFriendItem);
			}
		}

		private void OnGetMsg_chatviewSendMessage(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				this.OnSendMessageBtn(null);
			}
		}

		public void OnSendMessageBtn(GameObject go = null)
		{
			if (this.chatInput.value == string.Empty)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_MessageCanNotBeEmpty"), 1f);
				return;
			}
			if (this.selectid == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_MessageIsEmpty"), 1f);
				return;
			}
			if (go != null)
			{
				this._chatInput.OnSubmit();
			}
			FriendItem friendItem = this.selectFriendItem;
			if (this.chatInput.value == "\n")
			{
				return;
			}
			string value = this.chatInput.value;
			string nickName = ModelManager.Instance.Get_userData_X().NickName;
			long userId = long.Parse(ModelManager.Instance.Get_userData_X().UserId);
			int head = ModelManager.Instance.Get_userData_filed_X("Avatar");
			int headFrame = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
			int charmRankvalue = ModelManager.Instance.Get_userData_filed_X("CharmRankValue");
			AgentBaseInfo client = new AgentBaseInfo
			{
				NickName = nickName,
				UserId = userId,
				head = head,
				headFrame = headFrame,
				CharmRankvalue = charmRankvalue
			};
			if (!ToolsFacade.Instance.IsLegalString(ref value))
			{
				return;
			}
			ChatMessageNew data = new ChatMessageNew
			{
				Client = client,
				ChatType = 7,
				Message = value,
				TargetId = ToolsFacade.Instance.GetUserIdBySummId(long.Parse(this.selectid)).ToString(),
				TimeTick = ToolsFacade.ServerCurrentTime.Ticks
			};
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(100, SerializeHelper.Serialize<ChatMessageNew>(data));
			MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewSendChatToServer, dictionary, false);
			string text = NGUIText.StripSymbols(this._chatInput.mInput.value);
			text = FilterWorder.Instance.ReplaceKeyword(text);
			if (!string.IsNullOrEmpty(text))
			{
				AccountData accountData = ModelManager.Instance.Get_accountData_X();
				if (accountData != null)
				{
					this.AddLocalText(accountData.AccountId + "Message" + friendItem.id.ToString(), text, "me");
				}
				this._chatInput.mInput.isSelected = false;
				this.chatInput.value = string.Empty;
			}
		}

		private void OnSelect(string id)
		{
			this.ChatContentClear();
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(102, ToolsFacade.Instance.GetUserIdBySummId(long.Parse(id)).ToString());
			NetWorkHelper.Instance.client.SendSessionChannelMessage(7, MobaChannel.Chat, dictionary);
		}

		public void CheckselectObj(FriendItem _friendItem = null)
		{
			if (null == _friendItem)
			{
				this.TalkInfo.gameObject.SetActive(false);
				this.UserInfo.gameObject.SetActive(false);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(102, null);
				NetWorkHelper.Instance.client.SendSessionChannelMessage(7, MobaChannel.Chat, dictionary);
				return;
			}
			this.selectObj = _friendItem.gameObject;
			this.OnSelect(this.selectid);
			if (_friendItem == null)
			{
				return;
			}
			this.selectFriendItem = _friendItem;
			_friendItem.isSelect = true;
			_friendItem.HideMessageSign();
			if (this.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(_friendItem.id)))
			{
				this.newMessageList.Remove(ToolsFacade.Instance.GetUserIdBySummId(_friendItem.id));
			}
			if (_friendItem.friendType == 0 || _friendItem.friendType == 3)
			{
				this.TalkInfo.gameObject.SetActive(false);
				this.UserInfo.gameObject.SetActive(true);
				this.DelectFriend.gameObject.SetActive(false);
				this.MoveBlackBtn.gameObject.SetActive(true);
			}
			else if (_friendItem.friendType == 1)
			{
				this.TalkInfo.gameObject.SetActive(true);
				this.UserInfo.gameObject.SetActive(true);
				this.DelectFriend.gameObject.SetActive(true);
				this.CheckGameState(_friendItem);
				this.MoveBlackBtn.gameObject.SetActive(true);
			}
			else if (_friendItem.friendType == 2)
			{
				this.TalkInfo.gameObject.SetActive(false);
				this.UserInfo.gameObject.SetActive(true);
				this.DelectFriend.gameObject.SetActive(false);
				this.MoveBlackBtn.gameObject.SetActive(false);
			}
			this.SetUserInfo(_friendItem);
		}

		private void ChatContentAddTxt(string txt, int index = -1)
		{
			GameObject gameObject = null;
			UILabel uILabel = null;
			if (this.ChatContent == null)
			{
				this.ChatContent = this.TalkInfo.Find("Chat Window/ChatContent");
			}
			if (this.ChatContent == null)
			{
				return;
			}
			if (this.ChatContent.childCount > 0 && (index == -1 || index > 0))
			{
				if (index > 0 && index < this.ChatContent.childCount)
				{
					Transform transform = this.ChatContent.Find((index + 1).ToString());
					if (transform == null)
					{
						return;
					}
					gameObject = transform.gameObject;
					transform = this.ChatContent.Find(index.ToString());
					if (transform == null)
					{
						return;
					}
					uILabel = transform.GetComponent<UILabel>();
				}
				else
				{
					gameObject = NGUITools.AddChild(this.ChatContent.gameObject, this.firendChatTextPre);
					int activeChildCount = this.GetActiveChildCount();
					gameObject.name = activeChildCount.ToString();
					Transform transform = this.ChatContent.Find((activeChildCount - 1).ToString());
					if (transform != null)
					{
						uILabel = transform.GetComponent<UILabel>();
					}
				}
				if (uILabel != null)
				{
					gameObject.transform.localPosition = new Vector3(0f, uILabel.transform.localPosition.y - (float)uILabel.height, 0f);
				}
			}
			else
			{
				if (this.ChatContent.childCount == 0)
				{
					gameObject = NGUITools.AddChild(this.ChatContent.gameObject, this.firendChatTextPre);
				}
				else
				{
					Transform transform = this.ChatContent.Find((index + 1).ToString());
					if (transform != null)
					{
						gameObject = transform.gameObject;
					}
				}
				gameObject.name = 1.ToString();
				gameObject.transform.localPosition = Vector3.zero;
			}
			gameObject.gameObject.SetActive(true);
			UILabel component = gameObject.GetComponent<UILabel>();
			component.text = txt;
			this.ChatContentHight += component.height;
			if (this.ChatContentScrollView == null || this.ChatContentPanel == null)
			{
				return;
			}
			if (index == -1)
			{
				if ((float)this.ChatContentHight > this.ChatContentPanel.clipRange.w)
				{
					this.ChatContentScrollView.contentPivot = UIWidget.Pivot.Bottom;
				}
				else
				{
					this.ChatContentScrollView.contentPivot = UIWidget.Pivot.TopLeft;
				}
				this.ChatContentScrollView.ResetPosition();
			}
		}

		private int GetActiveChildCount()
		{
			int num = 0;
			for (int i = 0; i < this.ChatContent.childCount; i++)
			{
				if (this.ChatContent.GetChild(i).gameObject.activeInHierarchy)
				{
					num++;
				}
			}
			return num;
		}

		private void UpdateFriendTxt(FriendItem friendItem)
		{
			Dictionary<string, List<ChatMessageNew>> dictionary = ModelManager.Instance.Get_Friend_Chat_DataX();
			string text = ToolsFacade.Instance.GetUserIdBySummId(friendItem.id).ToString();
			if (dictionary.ContainsKey(text))
			{
				foreach (ChatMessageNew current in dictionary[text])
				{
					if (current.Client.UserId.ToString() == text)
					{
						this.ChatContentAddTxt("[ffffff]>" + friendItem.Name + ":[35abcb]" + current.Message, -1);
					}
					else
					{
						this.ChatContentAddTxt(LanguageManager.Instance.GetStringById("FriendsUI_Tips_Isay") + current.Message, -1);
					}
				}
			}
			if (this.FixScrollBarTask != null)
			{
				this.FixScrollBarTask.Stop();
			}
			if (this.coroutineManager == null)
			{
				this.coroutineManager = new CoroutineManager();
			}
			this.FixScrollBarTask = this.coroutineManager.StartCoroutine(this.FixScrollBar(), true);
		}

		[DebuggerHidden]
		private IEnumerator FixScrollBar()
		{
			FriendView.<FixScrollBar>c__Iterator129 <FixScrollBar>c__Iterator = new FriendView.<FixScrollBar>c__Iterator129();
			<FixScrollBar>c__Iterator.<>f__this = this;
			return <FixScrollBar>c__Iterator;
		}

		private void ChatContentClear()
		{
			while (this.ChatContent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(this.ChatContent.GetChild(0).gameObject);
			}
			this.ChatContentHight = 0;
		}

		public void AddMessage(string text)
		{
			long num = long.Parse(text);
			if (!this.newMessageList.Contains(num))
			{
				this.newMessageList.Add(num);
			}
			this.ShowMessageSign(num);
		}

		private void ShowMessageSign(long id)
		{
			string name = ToolsFacade.Instance.GetSummIdByUserid(id).ToString();
			if (null != this.FriendPanel && this.FriendPanel.transform.childCount > 0)
			{
				Transform transform = this.FriendPanel.Find(name);
				if (transform != null)
				{
					FriendItem component = transform.gameObject.GetComponent<FriendItem>();
					component.ShowMessageSign();
					this.StartGetFriendList();
				}
			}
		}

		private void AddFriendTxt(string id, string message, string time, int type)
		{
			if (type == 1)
			{
				this.ChatContentAddTxt("[ffffff]>" + this.selectFriendItem.Name + ":[35abcb]" + message, -1);
			}
			else if (type == 0)
			{
				this.ChatContentAddTxt(LanguageManager.Instance.GetStringById("FriendsUI_Tips_Isay") + message, -1);
			}
		}

		public void UpdateLocalText(string id, List<Messages> messagesList)
		{
			if (messagesList == null || messagesList.Count == 0)
			{
				return;
			}
			string text = string.Empty;
			for (int i = 1; i <= messagesList.Count; i++)
			{
				text = text + messagesList[messagesList.Count - i].Content + "∮" + messagesList[messagesList.Count - i].Time;
				if (i > 0 && i < messagesList.Count)
				{
					text += "∫";
				}
			}
			PlayerPrefs.SetString(id, this.EscapeURL(text));
			PlayerPrefs.Save();
		}

		private List<Messages> AddLocalMessagesList(string id, List<Messages> messagesList)
		{
			if (!PlayerPrefs.HasKey(id) || PlayerPrefs.GetString(id) == string.Empty)
			{
				return messagesList;
			}
			if (messagesList == null)
			{
				messagesList = new List<Messages>();
			}
			string text = this.UnEscapeURL(PlayerPrefs.GetString(id));
			string[] array = text.Split(new char[]
			{
				'∫'
			});
			int num;
			if (array.Length > 20)
			{
				num = array.Length - 20;
			}
			else
			{
				num = 0;
			}
			for (int i = array.Length - 1; i >= num; i--)
			{
				Messages messages = new Messages();
				string[] array2 = array[i].Split(new char[]
				{
					'∮'
				});
				messages.Content = array2[0];
				if (array2.Length > 1)
				{
					messages.Time = array2[1];
				}
				messagesList.Insert(0, messages);
			}
			return messagesList;
		}

		public void AddLocalText(string id, string content, string time)
		{
			string text = this.UnEscapeURL(PlayerPrefs.GetString(id));
			if (text != string.Empty)
			{
				text += "∫";
			}
			text = text + content + "∮" + time;
			PlayerPrefs.SetString(id, this.EscapeURL(text));
			PlayerPrefs.Save();
		}

		private string EscapeURL(string text)
		{
			text = WWW.EscapeURL(text);
			return text;
		}

		private string UnEscapeURL(string text)
		{
			text = WWW.UnEscapeURL(text);
			return text;
		}

		public void UpdateNewApply(string text = null)
		{
			if (this.gameObject != null && this.gameObject.activeInHierarchy)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_WaitingServerResponse"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
			}
			else
			{
				MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_OnFriendList));
				SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, null, new object[0]);
			}
			Singleton<MenuView>.Instance.SetNews(14, "0");
		}

		public void UpdateApplyState(string text = null, bool needHideRight = false)
		{
			string[] content = text.Split(new char[]
			{
				'|'
			});
			ModelManager.Instance.Get_FriendDataList_X().Remove(ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId.ToString() == content[1]));
			if (this.gameObject != null)
			{
				this.StartGetFriendList();
				if (needHideRight)
				{
					this.TalkInfo.gameObject.SetActive(false);
					this.UserInfo.gameObject.SetActive(false);
				}
			}
		}

		public void UpdateFriendState(string content)
		{
			string[] stateStrs = null;
			if (content == null || this.friendItemList == null)
			{
				return;
			}
			stateStrs = content.Split(new char[]
			{
				'|'
			});
			FriendItem friendItem = this.friendItemList.Find((FriendItem obj) => obj.id.ToString() == stateStrs[1]);
			if (friendItem == null || (int)friendItem.data.Status != 1)
			{
				return;
			}
			if (stateStrs == null || stateStrs.Length < 3)
			{
				return;
			}
			friendItem.SetGameState((GameStatus)int.Parse(stateStrs[2]));
			FriendData friendData = ModelManager.Instance.Get_FriendDataList_X().Find((FriendData _obj) => _obj.TargetId.ToString() == stateStrs[1]);
			if (friendData == null)
			{
				return;
			}
			friendData.GameStatus = (sbyte)int.Parse(stateStrs[2]);
			if (null == this.sendChatmessage)
			{
				return;
			}
			this.FixItemsPosition();
		}

		private void FixItemsPosition()
		{
			this.friendItemList.Sort(delegate(FriendItem left, FriendItem right)
			{
				if ((int)left.data.Status == 3)
				{
					return -2;
				}
				if ((int)left.data.GameStatus != 0 && (int)right.data.GameStatus != 0)
				{
					if ((int)right.data.GameStatus == 1 && (int)left.data.GameStatus != 1 && (int)left.data.GameStatus != 0)
					{
						return 1;
					}
					if ((int)left.data.GameStatus != (int)right.data.GameStatus)
					{
						return -1;
					}
					if (left.data.TargetId < right.data.TargetId)
					{
						return -1;
					}
					return 1;
				}
				else
				{
					if ((int)left.data.GameStatus == 0 && (int)right.data.GameStatus != 0)
					{
						return 1;
					}
					if ((int)left.data.GameStatus != (int)right.data.GameStatus)
					{
						return -1;
					}
					if (left.data.TargetId < right.data.TargetId)
					{
						return -1;
					}
					return 1;
				}
			});
			for (int i = 0; i < this.friendItemList.Count; i++)
			{
				this.friendItemList[i].transform.localPosition = new Vector3(0f, (float)(-176 * i), 0f);
			}
		}

		private void CheckGameState(FriendItem friendItem)
		{
			if (friendItem.gameState == 0)
			{
				this.OffLineWarn.gameObject.SetActive(true);
			}
			else
			{
				this.OffLineWarn.gameObject.SetActive(false);
			}
		}

		private void UpdateRadarFriends()
		{
			List<FriendData> list = ModelManager.Instance.Get_RadarFriendList_X();
			if (list == null || list.Count == 0)
			{
				return;
			}
			System.Random random = new System.Random();
			List<Vector2> list2 = this.RandomPoint();
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i].TargetName == ModelManager.Instance.Get_userData_X().NickName))
				{
					int index = random.Next(0, list2.Count - 1);
					GameObject gameObject = NGUITools.AddChild(this.FriendsList.gameObject, this.RadarFriendItem);
					if (list2 != null && list2.Count > 0)
					{
						gameObject.transform.localPosition = list2[index];
						list2.RemoveAt(index);
					}
					else
					{
						float x = (float)random.Next(-738, 739);
						float y = (float)random.Next(-389, 369);
						gameObject.transform.localPosition = new Vector3(x, y, 0f);
					}
					gameObject.transform.Find("AddFriend").gameObject.SetActive(false);
					gameObject.transform.Find("Friend").gameObject.SetActive(false);
					gameObject.transform.Find("HI~").gameObject.SetActive(false);
					gameObject.transform.Find("WaiteReq").gameObject.SetActive(false);
					gameObject.name = i.ToString();
					SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(list[i].Icon.ToString());
					gameObject.transform.Find("Headportrait").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById.headportrait_icon, true, true, null, 0, false);
					bool flag = this.IsFriend(list[i].TargetName);
					if (flag)
					{
						gameObject.transform.Find("Friend").gameObject.SetActive(true);
						UIEventListener.Get(gameObject).onClick = new UIEventListener.VoidDelegate(this.IsRadarFriend);
					}
					if (!flag)
					{
						UIEventListener.Get(gameObject).onClick = new UIEventListener.VoidDelegate(this.SendRadarReq);
						gameObject.transform.Find("AddFriend").gameObject.SetActive(true);
					}
				}
			}
		}

		private List<Vector2> RandomPoint()
		{
			Vector2[] array = new Vector2[]
			{
				new Vector2(311f, -119f),
				new Vector2(-294f, 70f),
				new Vector2(42f, -302f),
				new Vector2(164f, 272f),
				new Vector2(524f, -326f),
				new Vector2(-355f, 385f),
				new Vector2(-691f, -21f),
				new Vector2(463f, 101f),
				new Vector2(-343f, -198f)
			};
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(array[i]);
			}
			return list;
		}

		private void IsRadarFriend(GameObject go)
		{
			this.GetRadarInfo(int.Parse(go.name));
			this.RadarFriendStatu.spriteName = "Add_friend_icons_friend_02";
			this.RadarFriendStatuText.color = new Color32(31, 249, 116, 255);
			this.RadarFriendStatuText.text = "对方已经是你的好友";
		}

		private void SendRadarReq(GameObject go)
		{
			this.GetRadarInfo(int.Parse(go.name));
			this.RadarFriendStatu.spriteName = "Add_friend_icons_friend_01";
			this.RadarFriendStatuText.text = "加好友";
			this.RadarFriendStatuText.color = new Color32(15, 244, 215, 255);
			this.Idx = int.Parse(go.name);
			UIEventListener.Get(this.RadarFriendReq.transform.Find("Status").gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickSendReq);
		}

		private void ReceiveRadarReq(GameObject go)
		{
			this.GetRadarInfo(int.Parse(go.name));
			this.RadarFriendStatu.spriteName = "Add_friend_icons_friend_02";
			this.RadarFriendStatuText.color = new Color32(31, 249, 116, 255);
			this.RadarFriendStatuText.text = "同意";
			UIEventListener.Get(this.RadarFriendReq.transform.Find("Status").gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickAcceptReq);
		}

		private void IsSended(GameObject go)
		{
			this.GetRadarInfo(int.Parse(go.name));
			this.RadarFriendStatu.spriteName = "Add_friend_icons_friend_02";
			this.RadarFriendStatuText.color = new Color32(31, 249, 116, 255);
			this.RadarFriendStatuText.text = LanguageManager.Instance.GetStringById("BattleSettlement_HasBeenSend");
		}

		private void OnClickSendReq(GameObject go)
		{
			List<FriendData> list = ModelManager.Instance.Get_RadarFriendList_X();
			if (list.Count == 0 || list == null)
			{
				return;
			}
			if (!SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ApplyAddFriend, null, new object[]
			{
				list[this.Idx].SummId
			}))
			{
				Singleton<TipView>.Instance.ShowViewSetText(":(申请发送失败", 1f);
				return;
			}
			this.isRadarAdd = true;
			this.RadarFriendStatu.GetComponent<TweenScale>().ResetToBeginning();
			this.RadarFriendStatu.GetComponent<TweenScale>().PlayForward();
			this.RadarFriendStatu.transform.FindChild("fly").gameObject.SetActive(true);
			TweenPosition component = this.RadarFriendStatu.transform.FindChild("fly").GetComponent<TweenPosition>();
			component.ResetToBeginning();
			TweenAlpha component2 = this.RadarFriendStatu.transform.FindChild("fly").GetComponent<TweenAlpha>();
			component2.ResetToBeginning();
			component.PlayForward();
			component2.PlayForward();
			this.RadarFriendStatu.spriteName = "Add_friend_icons_friend_02";
			this.RadarFriendStatuText.color = new Color32(31, 249, 116, 255);
			this.RadarFriendStatuText.text = LanguageManager.Instance.GetStringById("BattleSettlement_HasBeenSend");
			UIEventListener.Get(this.RadarFriendStatu.gameObject).onClick = null;
			this.FriendsList.Find(this.Idx.ToString() + "/AddFriend").gameObject.SetActive(false);
			this.FriendsList.Find(this.Idx.ToString() + "/WaiteReq").gameObject.SetActive(true);
			UIEventListener.Get(this.FriendsList.Find(this.Idx.ToString()).gameObject).onClick = new UIEventListener.VoidDelegate(this.IsSended);
		}

		private void OnClickAcceptReq(GameObject go)
		{
		}

		private void GetRadarInfo(int i)
		{
			List<FriendData> list = ModelManager.Instance.Get_RadarFriendList_X();
			if (list == null || list.Count == 0)
			{
				return;
			}
			this.RadarFriendReq.gameObject.SetActive(true);
			SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(list[i].Icon.ToString());
			this.RadarFriendReq.transform.Find("Portrait").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById.headportrait_icon, true, true, null, 0, false);
			SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(list[i].PictureFrame.ToString());
			this.RadarFriendReq.transform.Find("Portrait/Sprite").transform.GetComponent<UISprite>().spriteName = dataById2.pictureframe_icon;
			this.RadarFriendReq.transform.Find("Name").GetComponent<UILabel>().text = list[i].TargetName;
			this.RadarFriendReq.transform.Find("Portrait/Label").GetComponent<UILabel>().text = CharacterDataMgr.instance.GetUserLevel(list[i].Exp).ToString();
			this.RadarFriendReq.transform.Find("MagicLv/Num").GetComponent<UILabel>().text = list[i].bottlelevel.ToString();
			this.RadarFriendReq.transform.Find("LadderLv/Num").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(this.GetState(list[i].LadderScore));
			this.RadarFriendReq.transform.Find("Charm/Num").GetComponent<UILabel>().text = list[i].charm.ToString();
			this.RadarFriendReq.transform.Find("MagicLv").GetComponent<UICenterHelper>().Reposition();
			this.RadarFriendReq.transform.Find("LadderLv").GetComponent<UICenterHelper>().Reposition();
			this.RadarFriendReq.transform.Find("Charm").GetComponent<UICenterHelper>().Reposition();
		}

		private void OnAddFirend(GameObject go)
		{
			this.FindPanelInputBg.height = 444;
			this.FindFriendItem.gameObject.SetActive(false);
			this.FindButton.gameObject.SetActive(true);
			this.QRScannerPanel.gameObject.SetActive(false);
			this.FindPanel.gameObject.SetActive(true);
			this.AddButton.gameObject.SetActive(false);
			this.FindPanel.GetComponent<UIPanel>().depth = Singleton<MenuTopBarView>.Instance.transform.GetComponent<UIPanel>().depth + 1;
			this.FriendPanel.GetComponent<UIPanel>().depth = Singleton<MenuTopBarView>.Instance.transform.GetComponent<UIPanel>().depth - 1;
			this.RadarPanel.gameObject.SetActive(false);
		}

		private void OnFindPanelBg(GameObject go)
		{
			this.FindPanel.gameObject.SetActive(false);
			this.FindFriendItem.gameObject.SetActive(false);
			this.FindButton.gameObject.SetActive(true);
			this.AddButton.gameObject.SetActive(false);
			this.findInput.value = string.Empty;
			this.Net.gameObject.SetActive(true);
			this.MyQR.gameObject.SetActive(true);
			this.QRScanner.gameObject.SetActive(true);
		}

		private void OnAddButton(GameObject go)
		{
			if (this.findInput.value == string.Empty)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_PleaseInputSummonerID"), 1f);
				return;
			}
			long num = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			if (this.findInput.value == num.ToString())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_CanNotAddYouself"), 1f);
				return;
			}
			if (ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId.ToString() == this.findInput.value) != null)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_HeIsYouFriend"), 1f);
				return;
			}
			if (this.findSelectId == -1L || !this.FindFriendItem.gameObject.activeInHierarchy)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_PleaseFindYourTargetFirst"), 1f);
				return;
			}
			this.AddButton.Find("Sprite").GetComponent<TweenScale>().PlayForward();
			this.AddButton.FindChild("fly").gameObject.SetActive(true);
			TweenPosition component = this.AddButton.transform.FindChild("fly").GetComponent<TweenPosition>();
			component.ResetToBeginning();
			TweenAlpha component2 = this.AddButton.transform.FindChild("fly").GetComponent<TweenAlpha>();
			component2.ResetToBeginning();
			component.PlayForward();
			component2.PlayForward();
			this.AddButton.Find("Sprite").GetComponent<UISprite>().spriteName = "Add_friend_icons_friend_02";
			this.AddButton.Find("Label").GetComponent<UILabel>().color = new Color32(31, 249, 116, 255);
			this.AddButton.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_HasBeenSend");
			UIEventListener.Get(this.AddButton.gameObject).onClick = null;
			this.findSelectId = long.Parse(this.findInput.value);
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Tips_Sending"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ApplyAddFriend, param, new object[]
			{
				this.findSelectId
			});
		}

		public void OnFindButton(GameObject go = null)
		{
			if (this.findInput.value == string.Empty)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_PleaseInputSummonerID"), 1f);
				return;
			}
			UIEventListener.Get(this.AddButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnAddButton);
			this.needShowFindFriendItem = true;
			long num = 0L;
			if (long.TryParse(this.findInput.value, out num))
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Tips_Submitting"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetUserInfoBySummId, param, new object[]
				{
					long.Parse(this.findInput.value)
				});
			}
		}

		public void OnFindButtonQR(string id)
		{
			this.coroutineManager.StartCoroutine(this.TryConnectGame(id), true);
		}

		[DebuggerHidden]
		private IEnumerator TryConnectGame(string id)
		{
			FriendView.<TryConnectGame>c__Iterator12A <TryConnectGame>c__Iterator12A = new FriendView.<TryConnectGame>c__Iterator12A();
			<TryConnectGame>c__Iterator12A.id = id;
			<TryConnectGame>c__Iterator12A.<$>id = id;
			<TryConnectGame>c__Iterator12A.<>f__this = this;
			return <TryConnectGame>c__Iterator12A;
		}

		private void OnQRScanner(GameObject go)
		{
			this.QRScannerPanel.gameObject.SetActive(true);
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			this.QRControl.Reset();
			this.QRPlane.gameObject.SetActive(true);
		}

		private void OnCancelBtn(GameObject go)
		{
			this.Net.gameObject.SetActive(true);
			this.MyQR.gameObject.SetActive(true);
			this.QRScanner.gameObject.SetActive(true);
			this.FindFriendItem.gameObject.SetActive(false);
			this.AddButton.gameObject.SetActive(false);
			this.FindButton.gameObject.SetActive(true);
		}

		private void OnDeleteBtn(GameObject go)
		{
			this.findInput.value = string.Empty;
			this.findInput.savedAs = string.Empty;
			this.Net.gameObject.SetActive(true);
			this.MyQR.gameObject.SetActive(true);
			this.QRScanner.gameObject.SetActive(true);
			this.FindFriendItem.gameObject.SetActive(false);
			this.AddButton.gameObject.SetActive(false);
			this.FindButton.gameObject.SetActive(true);
		}

		private void OnMyQR(GameObject go)
		{
			this.Net.gameObject.SetActive(false);
			this.MyQR.gameObject.SetActive(false);
			this.QRScanner.gameObject.SetActive(false);
			this.MyQRPanel.gameObject.SetActive(true);
			this.QRScannerPanel.gameObject.SetActive(false);
			this.GetQRTexture();
			this.MyQRName.text = ModelManager.Instance.Get_userData_X().NickName + "的二维码";
			this.MyQRPanel.GetComponent<UIPanel>().depth = Singleton<MenuTopBarView>.Instance.transform.GetComponent<UIPanel>().depth + 2;
			this.FriendPanel.GetComponent<UIPanel>().depth = Singleton<MenuTopBarView>.Instance.transform.GetComponent<UIPanel>().depth - 1;
		}

		private void OnRadarBtn(GameObject go = null)
		{
			this.RadarPanel.gameObject.SetActive(!this.RadarPanel.gameObject.activeInHierarchy);
			if (!this.RadarPanel.gameObject.activeInHierarchy)
			{
				while (this.FriendsList.childCount > 0)
				{
					UnityEngine.Object.DestroyImmediate(this.FriendsList.GetChild(0).gameObject);
				}
				return;
			}
			SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(ModelManager.Instance.Get_userData_X().Avatar.ToString());
			this.MyPortrait.mainTexture = ResourceManager.Load<Texture>(dataById.headportrait_icon, true, true, null, 0, false);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_RaderFindFriend, null, new object[0]);
		}

		private void OnCancelAddRadarBtn(GameObject go)
		{
			this.isRadarAdd = false;
			this.RadarFriendReq.gameObject.SetActive(false);
		}

		private void OnMyQRPanelBackBtn(GameObject go)
		{
			this.Net.gameObject.SetActive(true);
			this.MyQR.gameObject.SetActive(true);
			this.QRScanner.gameObject.SetActive(true);
			this.MyQRPanel.gameObject.SetActive(false);
		}

		private void OnQRScannerPanelBackBtn(GameObject go)
		{
			this.Net.gameObject.SetActive(true);
			this.MyQR.gameObject.SetActive(true);
			this.QRScanner.gameObject.SetActive(true);
			this.QRScannerPanel.gameObject.SetActive(false);
		}

		private void GetQRTexture()
		{
			long num = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			if (this.QRTexture == null)
			{
				this.QRTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
				this.QRTexture.name = "FriendView_GetQRTexture_" + Time.time.ToString();
				string textForEncoding = num.ToString();
				Color32[] pixels = this.QREncode(textForEncoding, this.QRTexture.width, this.QRTexture.height);
				this.QRTexture.SetPixels32(pixels);
				for (int i = 0; i < this.QRTexture.width; i++)
				{
					for (int j = 0; j < this.QRTexture.height; j++)
					{
						if (this.QRTexture.GetPixel(i, j) == Color.white)
						{
							this.QRTexture.SetPixel(i, j, this.myQRTexture.color);
						}
						else if (this.QRTexture.GetPixel(i, j) == Color.black)
						{
							this.QRTexture.SetPixel(i, j, Color.clear);
						}
					}
				}
				this.QRTexture.Apply();
			}
			this.myQRTexture.mainTexture = this.QRTexture;
			Texture2D mainTexture = null;
			SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(ModelManager.Instance.Get_userData_X().Avatar.ToString());
			if (dataById != null)
			{
				mainTexture = ResourceManager.Load<Texture2D>(dataById.headportrait_icon, true, true, null, 0, false);
			}
			this.headTex.mainTexture = mainTexture;
			this.myQRTexture.color = Color.white;
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

		private bool IsFriend(string name)
		{
			List<FriendData> list = ModelManager.Instance.Get_FriendDataList_X();
			return list != null && list.Count != 0 && list.Find((FriendData _obj) => _obj.TargetName == name) != null;
		}

		private void ChangeLabelText()
		{
			if (this.chatInput.value.Length > 28)
			{
				this.chatInput.isSelected = false;
				Singleton<TipView>.Instance.ShowViewSetText("最多输入28个字！", 1f);
				this.chatInput.value = this.chatInput.value.Remove(28);
			}
			this.chatInput.value = FilterWorder.Instance.ReplaceKeyword(this.chatInput.value);
			this.chatInput.GetComponent<UIInput>().value = this.chatInput.value;
		}
	}
}
