using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.HomeChatView
{
	public class FriendChat : MonoBehaviour
	{
		private Transform transFriendChat;

		private FriendItemInHomeChat friendItemPre;

		private object[] mgs;

		private Task listTask;

		private UIGrid mFriendList;

		private Transform returnBtn;

		private CoroutineManager coroutineManager = new CoroutineManager();

		private ChatItem myChatItem;

		private ChatItem otherChatItem;

		private Queue<ChatItem> queueChat;

		private Transform sendBtn;

		private UIInput InputStr;

		private UIGrid transGrid;

		private UIScrollView usv;

		private UIPanel usvPanel;

		private Transform mFriendChat;

		private Transform mFriendLst;

		private Transform EmojiBtn;

		public string selectFriendItem;

		private string lastSelect;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.chatviewChangeRoom,
				ClientC2V.ReceiveFriendChatMessage,
				ClientV2C.chatviewSendMessage,
				MobaChatCode.Chat_ListenPrivate,
				MobaChatCode.Chat_PullHistory,
				MobaFriendCode.Friend_GetFriendList
			};
			this.Initialize();
		}

		private void OnEnable()
		{
			this.Register();
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
			this.RefreshUI();
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.transFriendChat = base.transform.Find("FriendChat");
			this.mFriendChat = this.transFriendChat.Find("Chat");
			this.mFriendLst = this.transFriendChat.Find("FriendLst");
			this.mFriendList = this.transFriendChat.Find("FriendLst/Grid").GetComponent<UIGrid>();
			this.EmojiBtn = this.transFriendChat.Find("Chat/Normal/Emoj");
			this.returnBtn = this.transFriendChat.Find("Chat/ReturnToFriendList");
			this.sendBtn = this.transFriendChat.Find("Chat/Normal/SendBtn");
			this.myChatItem = Resources.Load<ChatItem>("Prefab/UI/HomeChat/MyChatText");
			this.otherChatItem = Resources.Load<ChatItem>("Prefab/UI/HomeChat/OthersChatText");
			this.usv = this.transFriendChat.Find("Chat/MessageBox").GetComponent<UIScrollView>();
			this.usvPanel = this.usv.GetComponent<UIPanel>();
			this.transGrid = this.transFriendChat.Find("Chat/MessageBox/Grid").GetComponent<UIGrid>();
			this.InputStr = this.transFriendChat.Find("Chat/InputField").GetComponent<UIInput>();
			UIEventListener.Get(this.returnBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ReturnToFriendLst);
			UIEventListener.Get(this.sendBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.SendFriendMsg);
			UIEventListener.Get(this.EmojiBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEmojiBtn);
			EventDelegate.Add(this.InputStr.onChange, new EventDelegate.Callback(this.LimitTextLength));
			this.queueChat = new Queue<ChatItem>();
		}

		private void RefreshUI()
		{
			this.mFriendChat.gameObject.SetActive(false);
			this.mFriendLst.gameObject.SetActive(true);
		}

		private void OnMsg_chatviewChangeRoom(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				ChitchatType chitchatType = (ChitchatType)((int)msg.Param);
				this.transFriendChat.gameObject.SetActive(chitchatType == ChitchatType.Friend);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(102, (chitchatType != ChitchatType.Friend) ? null : this.selectFriendItem);
				NetWorkHelper.Instance.client.SendSessionChannelMessage(7, MobaChannel.Chat, dictionary);
				if (chitchatType == ChitchatType.Friend)
				{
					SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
					SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
				}
			}
		}

		private void OnMsg_chatviewSendMessage(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				this.SendFriendMsg(null);
			}
		}

		private void OnMsg_ReceiveFriendChatMessage(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewCloseEmotion, null, false);
				ChatMessageNew chatMessageNew = (ChatMessageNew)msg.Param;
				if (chatMessageNew.Client.UserId == Singleton<HomeChatview>.Instance.UserID)
				{
					this.InputStr.value = string.Empty;
					this.InputStr.isSelected = false;
					this.DisplayData(chatMessageNew, true);
				}
				else if (chatMessageNew.Client.UserId.ToString() == this.selectFriendItem)
				{
					this.DisplayData(chatMessageNew, false);
				}
				this.SaveInModel(this.selectFriendItem, chatMessageNew);
			}
		}

		private void OnMsg_Chat_ListenPrivate(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					string text = (string)operationResponse.Parameters[102];
					if (text != null && text.Length > 0)
					{
						if (Singleton<FriendView>.Instance.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(long.Parse(text))))
						{
							Singleton<FriendView>.Instance.newMessageList.Remove(ToolsFacade.Instance.GetUserIdBySummId(long.Parse(text)));
							Singleton<HomeChatview>.Instance.UpdateNewsPoint();
							this.HideMessageSign(text);
						}
						Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
						dictionary.Add(102, text);
						NetWorkHelper.Instance.client.SendSessionChannelMessage(4, MobaChannel.Chat, dictionary);
					}
				}
			}
		}

		private void OnMsg_Chat_PullHistory(MobaMessage msg)
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
						this.DisplayData(chatMessageNew, false);
						this.SaveInModel(chatMessageNew.Client.UserId.ToString(), chatMessageNew);
					}
				}
			}
		}

		private void OnMsg_Friend_GetFriendList(MobaMessage msg)
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
				byte[] buffer = operationResponse.Parameters[27] as byte[];
				List<FriendData> dataList = SerializeHelper.Deserialize<List<FriendData>>(buffer);
				if (this.listTask != null)
				{
					this.listTask.Stop();
				}
				this.listTask = this.coroutineManager.StartCoroutine(this.StartGetFriendList(dataList), true);
			}
		}

		private void SaveInModel(string sender, ChatMessageNew chatMessage)
		{
			List<ChatMessageNew> list = ModelManager.Instance.SaveIn_Friend_Chat_MessageListX(sender, chatMessage);
		}

		private void GetFriendList(List<FriendData> _friendDataList)
		{
			if (this.listTask != null)
			{
				this.listTask.Stop();
			}
			this.listTask = this.coroutineManager.StartCoroutine(this.StartGetFriendList(_friendDataList), true);
		}

		[DebuggerHidden]
		private IEnumerator StartGetFriendList(List<FriendData> dataList)
		{
			FriendChat.<StartGetFriendList>c__Iterator155 <StartGetFriendList>c__Iterator = new FriendChat.<StartGetFriendList>c__Iterator155();
			<StartGetFriendList>c__Iterator.dataList = dataList;
			<StartGetFriendList>c__Iterator.<$>dataList = dataList;
			<StartGetFriendList>c__Iterator.<>f__this = this;
			return <StartGetFriendList>c__Iterator;
		}

		public void CreateList(List<FriendData> FriendDataList = null)
		{
			FriendDataList.Sort(delegate(FriendData x, FriendData y)
			{
				int num = 0;
				if (Singleton<FriendView>.Instance.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(x.TargetId)) && !Singleton<FriendView>.Instance.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(y.TargetId)))
				{
					num = -1;
				}
				if (!Singleton<FriendView>.Instance.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(x.TargetId)) && Singleton<FriendView>.Instance.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(y.TargetId)))
				{
					num = 1;
				}
				if (num == 0)
				{
					if ((int)x.GameStatus > (int)y.GameStatus)
					{
						num = -1;
					}
					else if ((int)x.GameStatus < (int)y.GameStatus)
					{
						num = 1;
					}
					else
					{
						num = (int)x.TargetId - (int)y.TargetId;
					}
				}
				return num;
			});
			FriendDataList = FriendDataList.FindAll((FriendData obj) => (int)obj.Status == 1);
			GridHelper.FillGrid<FriendItemInHomeChat>(this.mFriendList, this.friendItemPre, FriendDataList.Count, delegate(int idx, FriendItemInHomeChat comp)
			{
				comp.Initialize(FriendDataList[idx]);
				comp.transform.name = ToolsFacade.Instance.GetUserIdBySummId(FriendDataList[idx].TargetId).ToString();
				List<long> newMessageList = Singleton<FriendView>.Instance.newMessageList;
				string text = FriendDataList[idx].TargetId.ToString();
				if (Singleton<FriendView>.Instance.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(FriendDataList[idx].TargetId)))
				{
					comp.ShowMessageSign();
				}
				else
				{
					comp.HideMessageSign();
				}
			});
			this.mFriendList.repositionNow = true;
		}

		private void SendFriendMsg(GameObject go = null)
		{
			string nickName = ModelManager.Instance.Get_userData_X().NickName;
			long userId = long.Parse(ModelManager.Instance.Get_userData_X().UserId);
			int userLevel = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp);
			int ladder = ToolsFacade.Instance.ToInt32(ModelManager.Instance.Get_userData_X().LadderScore);
			int botLevel = ModelManager.Instance.Get_BottleData_Level();
			int head = ModelManager.Instance.Get_userData_filed_X("Avatar");
			int headFrame = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
			int charmRankvalue = ModelManager.Instance.Get_userData_filed_X("CharmRankValue");
			string value = this.InputStr.value;
			AgentBaseInfo client = new AgentBaseInfo
			{
				NickName = nickName,
				UserId = userId,
				head = head,
				headFrame = headFrame,
				Level = userLevel,
				Ladder = ladder,
				BotLevel = botLevel,
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
				TargetId = this.selectFriendItem,
				TimeTick = ToolsFacade.ServerCurrentTime.Ticks
			};
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(100, SerializeHelper.Serialize<ChatMessageNew>(data));
			SendMsgManager.SendMsgParam sendMsgParam = new SendMsgManager.SendMsgParam(false, "正在发送...", true, 15f);
			MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewSendChatToServer, dictionary, false);
		}

		private void DisplayData(ChatMessageNew chatMessage, bool isSelf)
		{
			string str = chatMessage.Message;
			AgentBaseInfo abi = chatMessage.Client;
			GridHelper.AddToGrid<ChatItem>(this.transGrid, (!isSelf) ? this.otherChatItem : this.myChatItem, isSelf, delegate(int idx, ChatItem comp)
			{
				comp.Init(abi, str, idx, isSelf, (ChatType)chatMessage.ChatType);
				this.queueChat.Enqueue(comp);
			});
			if (this.queueChat.Count > 99)
			{
				ChatItem chatItem = this.queueChat.Dequeue();
				UnityEngine.Object.Destroy(chatItem.gameObject);
			}
			this.usv.contentPivot = UIWidget.Pivot.Bottom;
			this.usv.ResetPosition();
			this.JudgePosition();
		}

		public void DisplayData(string message, bool isSelf)
		{
			AgentBaseInfo abi = new AgentBaseInfo();
			GridHelper.AddToGrid<ChatItem>(this.transGrid, (!isSelf) ? this.otherChatItem : this.myChatItem, isSelf, delegate(int idx, ChatItem comp)
			{
				comp.Init(abi, message, idx, isSelf, ChatType.PrivateMessage);
				this.queueChat.Enqueue(comp);
			});
			if (this.queueChat.Count > 99)
			{
				ChatItem chatItem = this.queueChat.Dequeue();
				UnityEngine.Object.Destroy(chatItem.gameObject);
			}
			this.usv.contentPivot = UIWidget.Pivot.Bottom;
			this.usv.ResetPosition();
			this.JudgePosition();
		}

		private void ReturnToFriendLst(GameObject go)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(102, null);
			NetWorkHelper.Instance.client.SendSessionChannelMessage(7, MobaChannel.Chat, dictionary);
			Singleton<HomeChatview>.Instance.FriendLst.gameObject.SetActive(true);
			Singleton<HomeChatview>.Instance.FriendChat.gameObject.SetActive(false);
			this.selectFriendItem = null;
			List<FriendData> friendDataList = ModelManager.Instance.Get_FriendDataList_X();
			this.CreateList(friendDataList);
		}

		public void AddFriendTxt(string id, string message, string time, int type)
		{
		}

		public void ShowMessageSign(string id)
		{
			Transform transform = this.mFriendList.transform.Find(id);
			if (transform != null)
			{
				FriendItemInHomeChat component = transform.gameObject.GetComponent<FriendItemInHomeChat>();
				component.ShowMessageSign();
			}
		}

		public void HideMessageSign(string id)
		{
			Transform transform = this.mFriendList.transform.Find(id);
			if (transform != null)
			{
				FriendItemInHomeChat component = transform.gameObject.GetComponent<FriendItemInHomeChat>();
				component.HideMessageSign();
			}
		}

		public void ClearChatBox()
		{
			while (this.transGrid.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(this.transGrid.transform.GetChild(0).gameObject);
			}
			this.LaodText();
		}

		private void ClickEmojiBtn(GameObject go)
		{
			if (null != go)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenEmotion, this.EmojiBtn.GetComponent<UISprite>(), false);
			}
		}

		private void LimitTextLength()
		{
			if (this.InputStr.value.Length > 28)
			{
				this.InputStr.isSelected = false;
				Singleton<TipView>.Instance.ShowViewSetText("最多输入28个字！", 1f);
				this.InputStr.value = this.InputStr.value.Remove(28);
			}
		}

		public void LaodText()
		{
			Dictionary<string, List<ChatMessageNew>> dictionary = ModelManager.Instance.Get_Friend_Chat_DataX();
			if (dictionary.ContainsKey(this.selectFriendItem))
			{
				foreach (ChatMessageNew current in dictionary[this.selectFriendItem])
				{
					if (current.Client.UserId.ToString() == this.selectFriendItem)
					{
						this.DisplayData(current, false);
					}
					else
					{
						this.DisplayData(current, true);
					}
				}
			}
		}

		private void CheckScrollPosition()
		{
			int childCount = this.transGrid.transform.childCount;
			if (childCount > 1)
			{
				if (Mathf.Abs(this.transGrid.transform.GetChild(childCount - 1).localPosition.y) >= this.usvPanel.GetViewSize().y)
				{
					this.usv.contentPivot = UIWidget.Pivot.Bottom;
				}
				else
				{
					this.usv.contentPivot = UIWidget.Pivot.Top;
				}
			}
			else
			{
				this.usv.contentPivot = UIWidget.Pivot.Top;
			}
			this.usv.ResetPosition();
		}

		private void JudgePosition()
		{
			int childCount = this.transGrid.transform.childCount;
			if (childCount < 6 && childCount > 0)
			{
				Transform child = this.transGrid.transform.GetChild(childCount - 1);
				child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y + 1f, child.localPosition.z);
			}
		}
	}
}
