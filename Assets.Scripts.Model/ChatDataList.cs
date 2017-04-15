using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class ChatDataList
	{
		public ChatUserInfo chatUserInfoData;

		public Queue<ChatMessageNew> HallChatData;

		public Dictionary<string, List<ChatMessageNew>> FriendChatData;

		public Queue<ChatMessageNew> LobbyChatData;

		public Queue<ChatMessageNew> tempHallData;

		public List<ChatMessageNew> vipHallData;

		public ChatDataList()
		{
			this.chatUserInfoData = new ChatUserInfo();
			this.HallChatData = new Queue<ChatMessageNew>();
			this.FriendChatData = new Dictionary<string, List<ChatMessageNew>>();
			this.LobbyChatData = new Queue<ChatMessageNew>();
			this.tempHallData = new Queue<ChatMessageNew>();
			this.vipHallData = new List<ChatMessageNew>();
		}
	}
}
