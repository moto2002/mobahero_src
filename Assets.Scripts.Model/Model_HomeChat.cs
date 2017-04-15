using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
	internal class Model_HomeChat : ModelBase<ChatDataList>
	{
		public static long LastMsgId;

		private List<MobaChatCode> listCode = new List<MobaChatCode>();

		private object[] mgs;

		private ChatDataList data;

		public Model_HomeChat()
		{
			base.Init(EModelType.Model_HomeChat);
			this.data = new ChatDataList();
			base.Data = this.data;
		}

		public override void RegisterMsgHandler()
		{
			this.mgs = new object[]
			{
				MobaChatCode.Chat_Send,
				MobaChatCode.Chat_Recv,
				MobaChatCode.Chat_Login,
				MobaChatCode.Chat_GetGlobleMsg
			};
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		public override void UnRegisterMsgHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			throw new NotImplementedException();
		}

		private void OnMsg_Chat_Login(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					byte[] buffer = (byte[])operationResponse.Parameters[100];
					ChatUserInfo chatUserInfo = SerializeHelper.Deserialize<ChatUserInfo>(buffer);
					if (chatUserInfo != null)
					{
						this.data.chatUserInfoData = chatUserInfo;
					}
				}
				base.Data = this.data;
			}
		}

		private void OnMsg_Chat_GetGlobleMsg(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte[] buffer = (byte[])operationResponse.Parameters[100];
			ChatMessagePullData chatMessagePullData = SerializeHelper.Deserialize<ChatMessagePullData>(buffer);
			if (chatMessagePullData != null && chatMessagePullData.messages != null)
			{
				foreach (ChatMessageNew current in chatMessagePullData.messages)
				{
					UserData userData = ModelManager.Instance.Get_userData_X();
					if (Model_HomeChat.LastMsgId == current.MessageId)
					{
						break;
					}
					MobaMessageManagerTools.SendClientMsg(ClientC2V.ReceiveHallChatMessage, current, false);
					Model_HomeChat.LastMsgId = current.MessageId;
					Debug.Log(string.Concat(new object[]
					{
						"OnMsgChat_GetGlobleMsg mes:",
						current.Message,
						"   id:",
						current.MessageId
					}));
				}
			}
		}

		private void OnMsg_Chat_Recv(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					byte[] buffer = (byte[])operationResponse.Parameters[100];
					ChatMessageNew chatMessageNew = SerializeHelper.Deserialize<ChatMessageNew>(buffer);
					UserData userData = ModelManager.Instance.Get_userData_X();
					if (userData.UserId.CompareTo(chatMessageNew.Client.UserId.ToString()) == 0 && chatMessageNew.ChatType == 3)
					{
						ModelManager.Instance.Set_Last_ChatSent_Time(ToolsFacade.ServerCurrentTime);
					}
					if (chatMessageNew.ChatType == 8 || chatMessageNew.ChatType == 5)
					{
						MobaMessageManagerTools.SendClientMsg(ClientC2V.ReceiveHallChatMessage, chatMessageNew, false);
						if (ModelManager.Instance.Get_HomeChatViewState())
						{
							this.data.tempHallData.Enqueue(chatMessageNew);
							if (this.data.tempHallData.Count > 40)
							{
								this.data.tempHallData.Dequeue();
							}
						}
						if (chatMessageNew.ChatType == 8 && this.data.vipHallData.Count < 40)
						{
							this.data.vipHallData.Add(chatMessageNew);
						}
						this.data.HallChatData.Enqueue(chatMessageNew);
						if (this.data.HallChatData.Count > 40)
						{
							this.data.HallChatData.Dequeue();
						}
					}
					else if (chatMessageNew.ChatType == 7)
					{
						MobaMessageManagerTools.SendClientMsg(ClientC2V.ReceiveFriendChatMessage, chatMessageNew, false);
					}
					else if (chatMessageNew.ChatType == 2)
					{
						MobaMessageManagerTools.SendClientMsg(ClientC2V.ReceiveRoomChatMessage, chatMessageNew, false);
						if (ModelManager.Instance.Get_HomeChatViewState())
						{
							this.data.LobbyChatData.Enqueue(chatMessageNew);
							if (this.data.tempHallData.Count > 40)
							{
								this.data.tempHallData.Dequeue();
							}
						}
					}
					base.Data = this.data;
				}
			}
		}

		private void OnMsg_Chat_Send(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					if (operationResponse.Parameters.ContainsKey(1))
					{
						object obj = operationResponse.Parameters[1];
					}
					UserData userData = ModelManager.Instance.Get_userData_X();
					if ((byte)operationResponse.Parameters[1] == 0)
					{
						byte b = (byte)operationResponse.Parameters[78];
						byte b2 = b;
						if (b2 != 3)
						{
							if (b2 == 8)
							{
								long num = (long)operationResponse.Parameters[61];
								userData.Speaker = (int)num;
							}
						}
						else
						{
							ModelManager.Instance.Set_Last_ChatSent_Time(ToolsFacade.ServerCurrentTime);
							MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewCountDown, null, false);
						}
					}
					else if ((byte)operationResponse.Parameters[1] == 10)
					{
						Singleton<TipView>.Instance.ShowViewSetText("小喇叭数量不足！！！", 1f);
					}
				}
			}
		}
	}
}
