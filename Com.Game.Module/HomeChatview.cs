using Assets.Scripts.GUILogic.View.HomeChatView;
using Assets.Scripts.Model;
using Com.Game.Utils;
using GUIFramework;
using MobaMessageData;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class HomeChatview : BaseView<HomeChatview>
	{
		private CoroutineManager coroutine;

		private Task task_tween;

		private Transform Anchor;

		private UIToggle hallToggle;

		private UILabel hallLabel;

		private UIToggle friendToggle;

		private UILabel friendLabel;

		private UIToggle lobbyToggle;

		private UILabel lobbyLabel;

		private Transform CloseBtn;

		private Transform BG;

		private Transform emojiBG;

		private Transform markPoint;

		private Transform friendNewsPoint;

		private Transform emojiList;

		private UIGrid emoGrid;

		private UIInput hallEmojiField;

		private UIInput friendEmojiField;

		private UIInput lobbyEmojiField;

		private UIDragScrollView dsv;

		private UIScrollView hallScrollView;

		private UIScrollView friendScrollView;

		private UIScrollView roomScrollView;

		private Color inActiveC = new Color(0f, 0.768627465f, 0.968627453f, 1f);

		private Color activeC = new Color(0.09019608f, 0.8f, 0.8f, 1f);

		private TweenPosition tween_position;

		private UISprite nowSprite;

		private EmojiItem emojiItem;

		public Transform FriendLst;

		public Transform FriendChat;

		private object[] mgs;

		private ChitchatType currType;

		public List<SessionUserData> mSessionUserData = new List<SessionUserData>();

		private Dictionary<ChitchatType, UIToggle> dicToggle = new Dictionary<ChitchatType, UIToggle>();

		private long userID;

		public FriendChat _friendChat;

		public ChatPlayerInfoController playerInfoController;

		private RoomChat _roomChat;

		public bool hasClearRoomChat = true;

		public long UserID
		{
			get
			{
				return this.userID;
			}
		}

		private ChitchatType CurrType
		{
			get
			{
				return this.currType;
			}
			set
			{
				this.currType = value;
				this.hallLabel.color = this.inActiveC;
				this.friendLabel.color = this.inActiveC;
				this.lobbyLabel.color = this.inActiveC;
				if (this.currType == ChitchatType.Hall)
				{
					this.hallLabel.color = this.activeC;
					this.markPoint.gameObject.SetActive(false);
					this.dsv.scrollView = this.hallScrollView;
				}
				else if (this.currType == ChitchatType.Friend)
				{
					this.friendLabel.color = this.activeC;
					this.dsv.scrollView = this.friendScrollView;
				}
				else if (this.currType == ChitchatType.Lobby)
				{
					this.lobbyLabel.color = this.activeC;
					this.dsv.scrollView = this.roomScrollView;
				}
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewChangeRoom, this.currType, false);
				this.dicToggle[this.currType].value = true;
				this.GetSetEmojiPanelState(false, this.nowSprite);
			}
		}

		public HomeChatview()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/HomeChat/HomeChatView");
		}

		public override void Init()
		{
			base.Init();
			this.mgs = new object[]
			{
				ClientV2C.chatviewInitRoom,
				ClientV2C.chatviewOpenEmotion,
				ClientV2C.chatviewCloseEmotion,
				ClientC2V.ReceiveHallChatMessage,
				ClientC2C.WaitServerResponseTimeOut
			};
			this.Anchor = this.transform.Find("Anchor");
			this.hallToggle = this.Anchor.Find("Panel/Lounge").GetComponent<UIToggle>();
			this.friendToggle = this.Anchor.Find("Panel/Friend").GetComponent<UIToggle>();
			this.lobbyToggle = this.Anchor.Find("Panel/Room").GetComponent<UIToggle>();
			this.hallLabel = this.hallToggle.transform.Find("Label").GetComponent<UILabel>();
			this.friendLabel = this.friendToggle.transform.Find("Label").GetComponent<UILabel>();
			this.lobbyLabel = this.lobbyToggle.transform.Find("Label").GetComponent<UILabel>();
			this.BG = this.Anchor.Find("BG");
			this.emojiBG = this.Anchor.Find("Panel/EmojiBG");
			this.CloseBtn = this.Anchor.Find("BG/CloseBtn");
			this.FriendLst = this.Anchor.Find("FriendChat/FriendLst");
			this.FriendChat = this.Anchor.Find("FriendChat/Chat");
			this._friendChat = this.Anchor.GetComponent<FriendChat>();
			this.emojiList = this.Anchor.Find("EmojiLst");
			this.emoGrid = this.emojiList.Find("Scroll View/Grid").GetComponent<UIGrid>();
			this.friendNewsPoint = this.Anchor.Find("Panel/Friend/Point");
			this._roomChat = this.Anchor.GetComponent<RoomChat>();
			this.playerInfoController = this.Anchor.GetComponent<ChatPlayerInfoController>();
			this.tween_position = this.transform.GetComponent<TweenPosition>();
			this.markPoint = this.Anchor.Find("Panel/Lounge/Mark");
			this.hallEmojiField = this.Anchor.Find("HallChat/LoudSpeaker/InputField").GetComponent<UIInput>();
			this.friendEmojiField = this.Anchor.Find("FriendChat/Chat/InputField").GetComponent<UIInput>();
			this.lobbyEmojiField = this.Anchor.Find("RoomChat/Chat/InputField").GetComponent<UIInput>();
			this.emojiItem = Resources.Load<EmojiItem>("Prefab/UI/HomeChat/emojiItem");
			this.dsv = this.Anchor.Find("BG").GetComponent<UIDragScrollView>();
			this.hallScrollView = this.Anchor.Find("HallChat/MessageBox").GetComponent<UIScrollView>();
			this.friendScrollView = this.Anchor.Find("FriendChat/Chat/MessageBox").GetComponent<UIScrollView>();
			this.roomScrollView = this.Anchor.Find("RoomChat/MessageBox").GetComponent<UIScrollView>();
			this.dicToggle[ChitchatType.Hall] = this.hallToggle;
			this.dicToggle[ChitchatType.Friend] = this.friendToggle;
			this.dicToggle[ChitchatType.Lobby] = this.lobbyToggle;
			EventDelegate.Add(this.hallToggle.onChange, new EventDelegate.Callback(this.ChangeToggle));
			EventDelegate.Add(this.friendToggle.onChange, new EventDelegate.Callback(this.ChangeToggle));
			EventDelegate.Add(this.lobbyToggle.onChange, new EventDelegate.Callback(this.ChangeToggle));
			UIEventListener.Get(this.CloseBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseChatView);
			this.userID = long.Parse(ModelManager.Instance.Get_userData_X().UserId);
			this.coroutine = new CoroutineManager();
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(102, null);
			NetWorkHelper.Instance.client.SendSessionChannelMessage(7, MobaChannel.Chat, dictionary);
		}

		public override void HandleAfterOpenView()
		{
			ModelManager.Instance.Set_HomeChatViewState(false);
			this.emojiList.gameObject.SetActive(false);
			Singleton<HomeChatview>.Instance._friendChat.selectFriendItem = null;
			this.UpdateNewsPoint();
			this.transform.GetComponent<UIPanel>().alpha = 0.01f;
			if (this.task_tween != null)
			{
				this.coroutine.StopCoroutine(this.task_tween);
			}
			this.task_tween = this.coroutine.StartCoroutine(this.OpenAnime(true), true);
			Singleton<MenuBottomBarView>.Instance.testLabel.text = string.Empty;
		}

		public override void HandleBeforeCloseView()
		{
			ModelManager.Instance.Set_HomeChatViewState(true);
			this.GetSetEmojiPanelState(false, this.nowSprite);
			if (this.coroutine != null)
			{
				this.coroutine.StopAllCoroutine();
			}
			this.task_tween = null;
		}

		private void ChangeToggle()
		{
			if (UIToggle.current.value)
			{
				if (this.playerInfoController != null)
				{
					this.playerInfoController.Hide();
				}
				foreach (KeyValuePair<ChitchatType, UIToggle> current in this.dicToggle)
				{
					if (current.Value == UIToggle.current)
					{
						this.CurrType = current.Key;
						break;
					}
				}
			}
		}

		private void OnMsg_ReceiveHallChatMessage(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				ChatMessageNew chatMessageNew = (ChatMessageNew)msg.Param;
				if (chatMessageNew.Client.UserId != this.userID && this.currType != ChitchatType.Hall)
				{
					this.markPoint.gameObject.SetActive(true);
				}
				else
				{
					this.markPoint.gameObject.SetActive(false);
				}
			}
		}

		private void OnMsg_chatviewOpenEmotion(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				UISprite sp = (UISprite)msg.Param;
				this.nowSprite = sp;
				this.ShowEmotionPanel(sp);
			}
		}

		private void OnMsg_chatviewCloseEmotion(MobaMessage msg)
		{
			if (msg != null)
			{
				this.emojiList.gameObject.SetActive(false);
				this.emojiBG.gameObject.SetActive(false);
			}
		}

		private void OnMsg_chatviewInitRoom(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				ChitchatType chitchatType = (ChitchatType)((int)msg.Param);
				this.CurrType = chitchatType;
				this.lobbyToggle.gameObject.SetActive(chitchatType == ChitchatType.Lobby);
			}
		}

		private void OnMsg_WaitServerResponseTimeOut(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageType mobaMessageType = MobaMessageType.ChatCode;
				int num = 2;
				MsgData_WaitServerResponsTimeout msgData_WaitServerResponsTimeout = (MsgData_WaitServerResponsTimeout)msg.Param;
				if (msgData_WaitServerResponsTimeout.MobaMsgType == mobaMessageType && msgData_WaitServerResponsTimeout.MsgID == num)
				{
					HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
				}
			}
		}

		private void CloseChatView(GameObject go)
		{
			if (null != this.transform && this.gameObject.activeInHierarchy)
			{
				if (this.task_tween != null)
				{
					this.coroutine.StopCoroutine(this.task_tween);
				}
				this.task_tween = this.coroutine.StartCoroutine(this.OpenAnime(false), true);
			}
		}

		private void ShowEmotionPanel(UISprite sp)
		{
			if (this.emojiList.gameObject.activeInHierarchy)
			{
				this.GetSetEmojiPanelState(false, sp);
			}
			else
			{
				this.GetSetEmojiPanelState(true, sp);
				if (this.emoGrid.transform.childCount > 0)
				{
					this.emoGrid.Reposition();
					GridHelper.FillGrid<EmojiItem>(this.emoGrid, this.emojiItem, this.emoGrid.transform.childCount, delegate(int idx, EmojiItem comp)
					{
						comp.ClickCallBack = new Callback<GameObject, EmojiItem>(this.FillEmojiToField);
					});
					return;
				}
				BetterList<string> emojiNameList = this.emojiItem.Sprite.atlas.GetListOfSprites();
				GridHelper.FillGrid<EmojiItem>(this.emoGrid, this.emojiItem, emojiNameList.size - 1, delegate(int idx, EmojiItem comp)
				{
					comp.Init(emojiNameList[idx]);
					comp.ClickCallBack = new Callback<GameObject, EmojiItem>(this.FillEmojiToField);
				});
			}
		}

		private void GetSetEmojiPanelState(bool isOn, UISprite sp = null)
		{
			if (null == sp || null == this.emojiList || null == this.emojiBG)
			{
				return;
			}
			sp.spriteName = ((!isOn) ? "Home_chatting_icons_emotion" : "Home_chatting_icons_emotion_02");
			this.emojiList.gameObject.SetActive(isOn);
			this.emojiBG.gameObject.SetActive(isOn);
		}

		private void FillEmojiToField(GameObject obj, EmojiItem ei)
		{
			if (null != obj)
			{
				ChitchatType chitchatType = this.currType;
				if (chitchatType != ChitchatType.Friend)
				{
					if (chitchatType == ChitchatType.Lobby)
					{
						UIInput expr_4D = this.lobbyEmojiField;
						expr_4D.value += ei.EmojiName;
					}
				}
				else
				{
					UIInput expr_2C = this.friendEmojiField;
					expr_2C.value += ei.EmojiName;
				}
				this.GetSetEmojiPanelState(false, this.nowSprite);
			}
		}

		public void AddFriendMessage(string content)
		{
			if (this._friendChat != null && !Singleton<FriendView>.Instance.newMessageList.Contains(long.Parse(content)))
			{
				Singleton<FriendView>.Instance.newMessageList.Add(long.Parse(content));
			}
			if (this.gameObject == null)
			{
				return;
			}
			if (this.FriendLst.gameObject.activeInHierarchy)
			{
				this.friendNewsPoint.gameObject.SetActive(true);
				List<FriendData> friendDataList = ModelManager.Instance.Get_FriendDataList_X();
				this._friendChat.CreateList(friendDataList);
				this._friendChat.ShowMessageSign(content);
			}
			this.friendNewsPoint.gameObject.SetActive(true);
		}

		public void UpdateFriendList(bool needNewLst, string _content = "")
		{
			if (Singleton<HomeChatview>.Instance.IsOpened)
			{
				if (needNewLst)
				{
					SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, null, new object[0]);
				}
				else
				{
					string[] content = _content.Split(new char[]
					{
						'|'
					});
					List<FriendData> list = ModelManager.Instance.Get_FriendDataList_X();
					FriendData item2 = list.Find((FriendData item) => item.SummId.ToString() == content[content.Length - 1]);
					list.Remove(item2);
					this._friendChat.CreateList(list);
				}
			}
		}

		public void UpdateFriendState(string content)
		{
			try
			{
				if (!string.IsNullOrEmpty(content))
				{
					string[] stateStrs = content.Split(new char[]
					{
						'|'
					});
					if (stateStrs.Length >= 3)
					{
						long userIdBySummId = ToolsFacade.Instance.GetUserIdBySummId(long.Parse(stateStrs[1]));
						FriendItemInHomeChat friendItemInHomeChat = this.FriendLst.transform.Find("Grid/" + userIdBySummId.ToString()).TryGetComp(null);
						if (!(friendItemInHomeChat == null))
						{
							friendItemInHomeChat.SetGameState((GameStatus)int.Parse(stateStrs[2]));
							List<FriendData> list = ModelManager.Instance.Get_FriendDataList_X();
							FriendData friendData = list.Find((FriendData _obj) => _obj.TargetId.ToString() == stateStrs[1]);
							if (friendData != null)
							{
								friendData.GameStatus = (sbyte)int.Parse(stateStrs[2]);
							}
							this._friendChat.CreateList(list);
						}
					}
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		public void UpdateNewsPoint()
		{
			if (Singleton<FriendView>.Instance.newMessageList.Count == 0)
			{
				this.friendNewsPoint.gameObject.SetActive(false);
			}
			else
			{
				this.friendNewsPoint.gameObject.SetActive(true);
			}
			Singleton<MenuView>.Instance.UpdateFriendNew();
		}

		public void UnFriended()
		{
			if (Singleton<FriendView>.Instance.newMessageList.Count == 0)
			{
				this.friendNewsPoint.gameObject.SetActive(false);
			}
			if (null != this.FriendChat.gameObject && this.FriendChat.gameObject.activeInHierarchy)
			{
				this.FriendChat.gameObject.SetActive(false);
				this.FriendLst.gameObject.SetActive(true);
				Singleton<HomeChatview>.Instance._friendChat.selectFriendItem = null;
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(102, null);
				NetWorkHelper.Instance.client.SendSessionChannelMessage(7, MobaChannel.Chat, dictionary);
			}
		}

		public void ClearRoomChat()
		{
			this.hasClearRoomChat = false;
			if (this._roomChat != null)
			{
				this._roomChat.ClearGrid(out this.hasClearRoomChat);
			}
		}

		[DebuggerHidden]
		private IEnumerator OpenAnime(bool isOpen)
		{
			HomeChatview.<OpenAnime>c__Iterator157 <OpenAnime>c__Iterator = new HomeChatview.<OpenAnime>c__Iterator157();
			<OpenAnime>c__Iterator.isOpen = isOpen;
			<OpenAnime>c__Iterator.<$>isOpen = isOpen;
			<OpenAnime>c__Iterator.<>f__this = this;
			return <OpenAnime>c__Iterator;
		}
	}
}
