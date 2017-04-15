using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Model
{
	public static class ModelTools_HomeChat
	{
		public static bool IsHomeChatViewClosed = true;

		public static ChatDataList GetChatDataList(this ModelManager mmng)
		{
			ChatDataList result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_HomeChat))
			{
				result = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			return result;
		}

		public static Queue<ChatMessageNew> Get_Hall_Chat_DataX(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			return chatDataList.HallChatData;
		}

		public static List<ChatMessageNew> Get_Hall_Chat_NonSelf_DataX(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			List<ChatMessageNew> list = new List<ChatMessageNew>();
			return chatDataList.HallChatData.ToList<ChatMessageNew>().FindAll((ChatMessageNew obj) => obj.Client.UserId != long.Parse(ModelManager.Instance.Get_userData_X().UserId));
		}

		public static List<ChatMessageNew> Get_Hall_Chat_VIP_DataX(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			List<ChatMessageNew> list = new List<ChatMessageNew>();
			return chatDataList.vipHallData;
		}

		public static ChatMessageNew Get_Hall_Chat_VIP_DataX(this ModelManager mmng, int index)
		{
			List<ChatMessageNew> list = mmng.Get_Hall_Chat_VIP_DataX();
			if (list == null || list.Count == 0)
			{
				return null;
			}
			if (index >= list.Count || list[index] == null)
			{
				return null;
			}
			return list[index];
		}

		public static Dictionary<string, List<ChatMessageNew>> Get_Friend_Chat_DataX(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			return chatDataList.FriendChatData;
		}

		public static List<ChatMessageNew> SaveIn_Friend_Chat_MessageListX(this ModelManager mmng, string senderID, ChatMessageNew chatMessage)
		{
			Dictionary<string, List<ChatMessageNew>> dictionary = mmng.Get_Friend_Chat_DataX();
			Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>();
			if (dictionary.Keys.Contains(senderID))
			{
				dictionary[senderID].Add(chatMessage);
			}
			else
			{
				dictionary.Add(senderID, new List<ChatMessageNew>
				{
					chatMessage
				});
			}
			return null;
		}

		public static Queue<ChatMessageNew> Get_Lobby_Chat_DataX(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			return chatDataList.LobbyChatData;
		}

		public static void Set_Lobby_Chat_Clear(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			chatDataList.LobbyChatData.Clear();
		}

		public static DateTime Get_Last_ChatSent_Time(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			DateTime result = new DateTime(chatDataList.chatUserInfoData.chatCheckTime);
			return result;
		}

		public static void Set_Last_ChatSent_Time(this ModelManager mmng, DateTime now)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			chatDataList.chatUserInfoData.chatCheckTime = now.Ticks;
		}

		public static void Set_HomeChatViewState(this ModelManager mmng, bool state)
		{
			ModelTools_HomeChat.IsHomeChatViewClosed = state;
		}

		public static bool Get_HomeChatViewState(this ModelManager mmng)
		{
			return ModelTools_HomeChat.IsHomeChatViewClosed;
		}

		public static Queue<ChatMessageNew> Get_TempHallChatView(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			return chatDataList.tempHallData;
		}

		public static void Set_ReSetTempHallChatView(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			chatDataList.tempHallData = new Queue<ChatMessageNew>();
			chatDataList.vipHallData = new List<ChatMessageNew>();
		}

		public static void Set_RemoveVipChatView(this ModelManager mmng)
		{
			ChatDataList chatDataList = mmng.GetChatDataList();
			if (chatDataList == null)
			{
				chatDataList = mmng.GetData<ChatDataList>(EModelType.Model_HomeChat);
			}
			chatDataList.vipHallData.RemoveAt(0);
		}
	}
}
