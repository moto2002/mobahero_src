using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.HomeChatView
{
	public class HallChat : MonoBehaviour
	{
		private const float CHAT_CD_TIME = 60f;

		private Transform transHallChat;

		private Transform transBlock;

		private Transform sendBtn;

		private Transform emojiList;

		private Transform broadcastBtn;

		private Transform emojiBG;

		private UIScrollView usv;

		private UIPanel usvPanel;

		private UIGrid transGrid;

		private UIGrid emoGrid;

		private UIInput content;

		private UIScrollBar bar;

		private UISprite emojiBtn;

		private ChatItem myChatItem;

		private ChatItem otherChatItem;

		private EmojiItem emojiItem;

		private Queue<ChatItem> queueChat;

		private ChitchatType ccType;

		private BoxCollider boc;

		private Task task_countDown;

		private CoroutineManager coroutine;

		private Coroutine monoCoroutine;

		private object[] mgs;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.chatviewChangeRoom,
				ClientV2C.chatviewSendMessage,
				ClientV2C.chatviewFillChatHistory,
				ClientC2V.ReceiveHallChatMessage,
				ClientV2C.chatviewCountDown
			};
			this.Initialize();
		}

		private void OnEnable()
		{
			this.Register();
			HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
			base.InvokeRepeating("SendGetGlobleChatMsg", 0.5f, 3f);
		}

		private void OnDisable()
		{
			this.Unregister();
			this.GetSetEmojiPanelState(false);
			base.CancelInvoke();
		}

		private void OnApplicationPause(bool isPause)
		{
			if (!isPause)
			{
			}
			this.CheckInputState(true);
		}

		private void OnDestroy()
		{
			if (this.coroutine != null)
			{
				this.coroutine.StopAllCoroutine();
			}
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
			MobaMessageManager.RegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.RegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
			MobaMessageManager.UnRegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.UnRegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (this.content.value.Length > 28)
			{
				this.content.isSelected = false;
				Singleton<TipView>.Instance.ShowViewSetText("最多输入28个字！", 1f);
				this.content.value = this.content.value.Remove(28);
			}
		}

		private void Initialize()
		{
			this.transHallChat = base.transform.Find("HallChat");
			this.sendBtn = this.transHallChat.Find("LoudSpeaker/SendBtn");
			this.emojiBtn = this.transHallChat.Find("LoudSpeaker/Emoji").GetComponent<UISprite>();
			this.broadcastBtn = this.transHallChat.Find("LoudSpeaker/Turmpet");
			this.transBlock = this.transHallChat.Find("LoudSpeaker/Input");
			this.usv = this.transHallChat.Find("MessageBox").GetComponent<UIScrollView>();
			this.usvPanel = this.usv.GetComponent<UIPanel>();
			this.transGrid = this.transHallChat.Find("MessageBox/Grid").GetComponent<UIGrid>();
			this.content = this.transHallChat.Find("LoudSpeaker/InputField").GetComponent<UIInput>();
			this.myChatItem = Resources.Load<ChatItem>("Prefab/UI/HomeChat/MyChatText");
			this.otherChatItem = Resources.Load<ChatItem>("Prefab/UI/HomeChat/OthersChatText");
			this.emojiItem = Resources.Load<EmojiItem>("Prefab/UI/HomeChat/emojiItem");
			this.bar = this.transHallChat.Find("ScrollBar").GetComponent<UIScrollBar>();
			this.emojiList = base.transform.Find("EmojiLst");
			this.emoGrid = this.emojiList.Find("Scroll View/Grid").GetComponent<UIGrid>();
			this.emojiBG = base.transform.Find("Panel/EmojiBG");
			this.coroutine = new CoroutineManager();
			this.queueChat = new Queue<ChatItem>();
			UIEventListener.Get(this.sendBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.SendChat);
			UIEventListener.Get(this.emojiBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowEmotionPanel);
			UIEventListener.Get(this.broadcastBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowTrumpetView);
			EventDelegate.Add(this.content.onChange, new EventDelegate.Callback(this.IsChange));
			this.content.label.color = new Color32(0, 85, 134, 255);
			this.content.defaultText = "点此输入文字";
			this.boc = this.content.GetComponent<BoxCollider>();
			this.GetSetCollider(false);
		}

		private void OnMsg_chatviewChangeRoom(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				ChitchatType chitchatType = (ChitchatType)((int)msg.Param);
				this.ccType = chitchatType;
				this.GetSetEmojiPanelState(false);
				this.transHallChat.gameObject.SetActive(chitchatType == ChitchatType.Hall);
				this.usv.contentPivot = UIWidget.Pivot.Bottom;
				this.usv.ResetPosition();
				this.JudgePosition();
				if (this.task_countDown != null && this.task_countDown.Running)
				{
					return;
				}
				this.CheckInputState(chitchatType == ChitchatType.Hall);
			}
		}

		private void OnMsg_chatviewSendMessage(MobaMessage msg)
		{
			if (msg != null)
			{
				this.SendChat(base.gameObject);
			}
		}

		private void OnMsg_chatviewFillChatHistory(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				bool flag = (bool)msg.Param;
				long num = long.Parse(ModelManager.Instance.Get_userData_X().UserId);
				Queue<ChatMessageNew> source = new Queue<ChatMessageNew>();
				source = ModelManager.Instance.Get_TempHallChatView();
				List<ChatMessageNew> chatMessageArray = (!flag) ? source.ToList<ChatMessageNew>() : ModelManager.Instance.Get_Hall_Chat_DataX().ToList<ChatMessageNew>();
				int i;
				for (i = 0; i != chatMessageArray.Count; i++)
				{
					string str = chatMessageArray[i].Message;
					AgentBaseInfo abi = chatMessageArray[i].Client;
					if (chatMessageArray[i].ChatType == 8)
					{
						str = "#e062" + str;
					}
					bool isSelf = chatMessageArray[i].Client.UserId == num;
					GridHelper.AddToGrid<ChatItem>(this.transGrid, (!isSelf) ? this.otherChatItem : this.myChatItem, isSelf, delegate(int idx, ChatItem comp)
					{
						comp.Init(abi, str, idx, isSelf, (ChatType)chatMessageArray[i].ChatType);
						comp.name = idx.ToString();
						this.queueChat.Enqueue(comp);
					});
					if (this.queueChat.Count > 40)
					{
						ChatItem chatItem = this.queueChat.Dequeue();
						UnityEngine.Object.Destroy(chatItem.gameObject);
					}
				}
				this.usv.contentPivot = UIWidget.Pivot.Bottom;
				this.usv.ResetPosition();
				this.JudgePosition();
				ModelManager.Instance.Set_ReSetTempHallChatView();
				HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
			}
		}

		private void OnPeerConnected(MobaMessage msg)
		{
			this.CheckInputState(true);
			HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
		}

		private void OnPeerDisconnected(MobaMessage msg)
		{
			this.CheckInputState(true);
			HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
		}

		private void OnMsg_ReceiveHallChatMessage(MobaMessage msg)
		{
			if (msg != null)
			{
				ChatMessageNew chatMessageNew = (ChatMessageNew)msg.Param;
				if (chatMessageNew.Client.UserId == Singleton<HomeChatview>.Instance.UserID)
				{
					this.DisplayData(chatMessageNew, true);
				}
				else
				{
					this.DisplayData(chatMessageNew, false);
				}
				HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
			}
		}

		private void OnMsg_chatviewCountDown(MobaMessage msg)
		{
			HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
			this.content.value = string.Empty;
			this.content.isSelected = false;
			this.content.label.color = Color.white;
			this.content.defaultText = "[d60d0d]60秒[-][005586]后方可输入[-]";
			this.CheckInputState(true);
		}

		private void IsChange()
		{
			if (this.content.isSelected)
			{
				this.content.label.supportEncoding = false;
			}
			else if (!string.IsNullOrEmpty(this.content.value) && this.content.value.Length >= 5)
			{
				string text = this.content.value.Substring(this.content.value.Length - 5);
				if (text.Contains("#e"))
				{
					this.content.label.supportEncoding = false;
				}
			}
			else
			{
				this.content.label.supportEncoding = true;
			}
		}

		private void SendChat(GameObject obj)
		{
			if (HomeChatCtrl.GetInstance().sendState == SendState.Sending)
			{
				return;
			}
			if (this.task_countDown != null && this.task_countDown.Running)
			{
				Singleton<TipView>.Instance.ShowViewSetText("休息一下 稍后再发", 1f);
				return;
			}
			HomeChatCtrl.GetInstance().sendState = SendState.Sending;
			this.GetSetEmojiPanelState(false);
			string nickName = ModelManager.Instance.Get_userData_X().NickName;
			long userId = long.Parse(ModelManager.Instance.Get_userData_X().UserId);
			int userLevel = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp);
			int ladder = ToolsFacade.Instance.ToInt32(ModelManager.Instance.Get_userData_X().LadderScore);
			int botLevel = ModelManager.Instance.Get_BottleData_Level();
			int head = ModelManager.Instance.Get_userData_filed_X("Avatar");
			int headFrame = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
			int charmRankvalue = ModelManager.Instance.Get_userData_filed_X("CharmRankValue");
			string value = this.content.value;
			bool flag = false;
			if (!string.IsNullOrEmpty(value) && value.Length > 1 && value[0] == '/')
			{
				flag = true;
			}
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
			if (!flag)
			{
				if (!ToolsFacade.Instance.IsLegalString(ref value))
				{
					UnityEngine.Debug.LogError("return");
					return;
				}
			}
			ChatMessageNew data = new ChatMessageNew
			{
				Client = client,
				ChatType = 3,
				Message = value,
				TargetId = null,
				TimeTick = ToolsFacade.ServerCurrentTime.Ticks
			};
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(100, SerializeHelper.Serialize<ChatMessageNew>(data));
			dictionary.Add(5, Model_HomeChat.LastMsgId);
			MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewSendChatToServer, dictionary, false);
		}

		private void SendGetGlobleChatMsg()
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(5, Model_HomeChat.LastMsgId);
			NetWorkHelper.Instance.client.SendSessionChannelMessage(9, MobaChannel.Chat, dictionary);
		}

		private void ShowTrumpetView(GameObject obj)
		{
			if (null != obj)
			{
				CtrlManager.OpenWindow(WindowID.TrumpetView, null);
			}
		}

		private void DisplayData(ChatMessageNew chatMessage, bool isSelf)
		{
			string str = chatMessage.Message;
			AgentBaseInfo abi = chatMessage.Client;
			if (chatMessage.ChatType == 8)
			{
				str = "#e062" + str;
				if (Singleton<TrumpetView>.Instance != null && isSelf)
				{
					Singleton<TrumpetView>.Instance.ClearContent();
				}
				CtrlManager.CloseWindow(WindowID.TrumpetView);
			}
			GridHelper.AddToGrid<ChatItem>(this.transGrid, (!isSelf) ? this.otherChatItem : this.myChatItem, isSelf, delegate(int idx, ChatItem comp)
			{
				comp.Init(abi, str, idx, isSelf, (ChatType)chatMessage.ChatType);
				comp.name = idx.ToString();
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

		private void ShowEmotionPanel(GameObject obj)
		{
			if (null != obj)
			{
				if (this.emojiList.gameObject.activeInHierarchy)
				{
					this.GetSetEmojiPanelState(false);
				}
				else
				{
					this.GetSetEmojiPanelState(true);
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
		}

		private void GetSetCollider(bool isOpen)
		{
			if (null != this.boc)
			{
				this.boc.enabled = isOpen;
			}
		}

		private void GetSetEmojiPanelState(bool isOn)
		{
			if (null == this.emojiBtn || null == this.emojiList || null == this.emojiBG)
			{
				return;
			}
			this.emojiBtn.spriteName = ((!isOn) ? "Home_chatting_icons_emotion" : "Home_chatting_icons_emotion_02");
			this.emojiList.gameObject.SetActive(isOn);
			this.emojiBG.gameObject.SetActive(isOn);
		}

		private void FillEmojiToField(GameObject obj, EmojiItem ei)
		{
			if (null != obj)
			{
				if ((this.task_countDown != null && !this.task_countDown.Running) || this.task_countDown == null)
				{
					UIInput expr_38 = this.content;
					expr_38.value += ei.EmojiName;
				}
				this.GetSetEmojiPanelState(false);
			}
		}

		private void CheckInputState(bool isCheck = true)
		{
			if (!isCheck)
			{
				return;
			}
			TimeSpan timeSpan = this.SinceLastChatSent();
			if (timeSpan.TotalSeconds < 60.0)
			{
				if (this.task_countDown != null)
				{
					this.coroutine.StopCoroutine(this.task_countDown);
				}
				this.task_countDown = this.coroutine.StartCoroutine(this.CountDown(timeSpan.TotalSeconds), true);
				this.GetSetCollider(false);
			}
			else
			{
				if (this.coroutine != null)
				{
					if (this.task_countDown != null)
					{
						this.coroutine.StopCoroutine(this.task_countDown);
					}
					else
					{
						this.coroutine.StopAllCoroutine();
					}
				}
				this.content.label.color = new Color32(0, 85, 134, 255);
				this.content.defaultText = "点此输入文字";
				this.GetSetCollider(true);
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
					this.usv.ResetPosition();
					this.JudgePosition();
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
		}

		private TimeSpan SinceLastChatSent()
		{
			DateTime serverCurrentTime = ToolsFacade.ServerCurrentTime;
			DateTime d = ModelManager.Instance.Get_Last_ChatSent_Time();
			TimeSpan result;
			if (d.Ticks == 0L)
			{
				result = TimeSpan.MaxValue;
			}
			else
			{
				result = serverCurrentTime - d;
			}
			return result;
		}

		[DebuggerHidden]
		private IEnumerator CountDown(double cd)
		{
			HallChat.<CountDown>c__Iterator156 <CountDown>c__Iterator = new HallChat.<CountDown>c__Iterator156();
			<CountDown>c__Iterator.cd = cd;
			<CountDown>c__Iterator.<$>cd = cd;
			<CountDown>c__Iterator.<>f__this = this;
			return <CountDown>c__Iterator;
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
