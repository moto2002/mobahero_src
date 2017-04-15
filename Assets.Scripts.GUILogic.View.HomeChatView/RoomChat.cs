using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.HomeChatView
{
	public class RoomChat : MonoBehaviour
	{
		public GameObject mRoomChatRoot;

		public GameObject mSendChat;

		public GameObject mEmojiBtn;

		public UIGrid mChatGrid;

		public UIScrollView mScrollView;

		public UIInput mChatInput;

		private UIPanel mViewPanel;

		private ChatItem selfChatItem;

		private ChatItem normalChatItem;

		private Queue<ChatItem> queueChat;

		private void Awake()
		{
			this.InitializeUIComp();
		}

		private void OnEnable()
		{
			this.queueChat.Clear();
			this.Register();
		}

		private void OnDisable()
		{
			this.UnRegister();
		}

		private void Update()
		{
			if (this.mChatInput.value.Length > 28)
			{
				this.mChatInput.isSelected = false;
				Singleton<TipView>.Instance.ShowViewSetText("最多输入28个字！", 1f);
				this.mChatInput.value = this.mChatInput.value.Remove(28);
			}
			if (base.gameObject.activeInHierarchy && (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return)))
			{
				Singleton<TrumpetView>.Instance.ConfirmSendMessage(base.gameObject);
			}
		}

		private void InitializeUIComp()
		{
			this.mViewPanel = this.mScrollView.GetComponent<UIPanel>();
			this.queueChat = new Queue<ChatItem>();
			this.selfChatItem = Resources.Load<ChatItem>("Prefab/UI/HomeChat/MyChatText");
			this.normalChatItem = Resources.Load<ChatItem>("Prefab/UI/HomeChat/OthersChatText");
			UIEventListener.Get(this.mSendChat).onClick = new UIEventListener.VoidDelegate(this.OnClick_Send);
			UIEventListener.Get(this.mEmojiBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_Emoji);
		}

		private void Register()
		{
			MobaMessageManager.RegistMessage((ClientMsg)21066, new MobaMessageFunc(this.OnMsg_chatviewInitRoom));
			MobaMessageManager.RegistMessage((ClientMsg)21068, new MobaMessageFunc(this.OnMsg_chatviewChangeRoom));
			MobaMessageManager.RegistMessage((ClientMsg)21069, new MobaMessageFunc(this.OnMsg_chatviewSendMessage));
			MobaMessageManager.RegistMessage((ClientMsg)23062, new MobaMessageFunc(this.OnMsg_ReceiveRoomChatMessage));
		}

		private void UnRegister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)21066, new MobaMessageFunc(this.OnMsg_chatviewInitRoom));
			MobaMessageManager.UnRegistMessage((ClientMsg)21068, new MobaMessageFunc(this.OnMsg_chatviewChangeRoom));
			MobaMessageManager.UnRegistMessage((ClientMsg)21069, new MobaMessageFunc(this.OnMsg_chatviewSendMessage));
			MobaMessageManager.UnRegistMessage((ClientMsg)23062, new MobaMessageFunc(this.OnMsg_ReceiveRoomChatMessage));
		}

		private void ClearChatItems()
		{
		}

		private void SendChat()
		{
			string nickName = ModelManager.Instance.Get_userData_X().NickName;
			long userId = long.Parse(ModelManager.Instance.Get_userData_X().UserId);
			int userLevel = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp);
			int ladder = ToolsFacade.Instance.ToInt32(ModelManager.Instance.Get_userData_X().LadderScore);
			int botLevel = ModelManager.Instance.Get_BottleData_Level();
			int head = ModelManager.Instance.Get_userData_filed_X("Avatar");
			int headFrame = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
			int charmRankvalue = ModelManager.Instance.Get_userData_filed_X("CharmRankValue");
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
			string value = this.mChatInput.value;
			if (!ToolsFacade.Instance.IsLegalString(ref value))
			{
				return;
			}
			ChatMessageNew data = new ChatMessageNew
			{
				Client = client,
				ChatType = 2,
				Message = value,
				TargetId = null,
				TimeTick = ToolsFacade.ServerCurrentTime.Ticks
			};
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(100, SerializeHelper.Serialize<ChatMessageNew>(data));
			MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewSendChatToServer, dictionary, false);
			this.mChatInput.value = null;
		}

		private void DisplayData(ChatMessageNew chatMessage, bool isSelf)
		{
			string str = chatMessage.Message;
			AgentBaseInfo abi = chatMessage.Client;
			GridHelper.AddToGrid<ChatItem>(this.mChatGrid, (!isSelf) ? this.normalChatItem : this.selfChatItem, isSelf, delegate(int idx, ChatItem comp)
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
		}

		public void ClearGrid(out bool hasClear)
		{
			if (this.mChatGrid != null)
			{
				for (int i = 0; i < this.mChatGrid.transform.childCount; i++)
				{
					UnityEngine.Object.Destroy(this.mChatGrid.transform.GetChild(i).gameObject);
				}
				hasClear = true;
			}
			else
			{
				hasClear = false;
			}
		}

		private void OnMsg_chatviewInitRoom(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				if ((int)msg.Param == 3)
				{
					Queue<ChatMessageNew> queue = ModelManager.Instance.Get_Lobby_Chat_DataX();
					if (!Singleton<HomeChatview>.Instance.hasClearRoomChat)
					{
						this.ClearGrid(out Singleton<HomeChatview>.Instance.hasClearRoomChat);
					}
					while (queue.Count > 0)
					{
						ChatMessageNew chatMessageNew = queue.Dequeue();
						bool isSelf = chatMessageNew.Client.UserId == Singleton<HomeChatview>.Instance.UserID;
						this.DisplayData(chatMessageNew, isSelf);
					}
					this.mRoomChatRoot.SetActive(true);
					this.mScrollView.ResetPosition();
					this.JudgePosition();
				}
				else
				{
					this.mRoomChatRoot.SetActive(false);
				}
			}
		}

		private void OnMsg_chatviewChangeRoom(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				ChitchatType chitchatType = (ChitchatType)((int)msg.Param);
				this.mRoomChatRoot.SetActive(chitchatType == ChitchatType.Lobby);
			}
		}

		private void OnMsg_chatviewSendMessage(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				this.SendChat();
			}
		}

		private void OnMsg_ReceiveRoomChatMessage(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewCloseEmotion, null, false);
				ChatMessageNew chatMessageNew = (ChatMessageNew)msg.Param;
				bool isSelf = chatMessageNew.Client.UserId == Singleton<HomeChatview>.Instance.UserID;
				this.DisplayData(chatMessageNew, isSelf);
				this.mScrollView.ResetPosition();
				this.JudgePosition();
			}
		}

		private void OnClick_Emoji(GameObject obj = null)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenEmotion, this.mEmojiBtn.GetComponent<UISprite>(), false);
		}

		private void OnClick_Send(GameObject obj = null)
		{
			this.SendChat();
		}

		private void CheckScrollPosition()
		{
			int childCount = this.mChatGrid.transform.childCount;
			if (childCount > 1)
			{
				if (Mathf.Abs(this.mChatGrid.transform.GetChild(childCount - 1).localPosition.y) >= this.mViewPanel.GetViewSize().y)
				{
					this.mScrollView.contentPivot = UIWidget.Pivot.Bottom;
				}
				else
				{
					this.mScrollView.contentPivot = UIWidget.Pivot.Top;
				}
			}
			else
			{
				this.mScrollView.contentPivot = UIWidget.Pivot.Top;
			}
			this.mScrollView.ResetPosition();
		}

		private void JudgePosition()
		{
			int childCount = this.mChatGrid.transform.childCount;
			if (childCount < 6 && childCount > 0)
			{
				Transform child = this.mChatGrid.transform.GetChild(childCount - 1);
				child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y + 1f, child.localPosition.z);
			}
		}
	}
}
