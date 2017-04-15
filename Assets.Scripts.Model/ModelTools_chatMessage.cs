using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_chatMessage
	{
		public static List<ChatMessage> Get_cahtMessagelist_X(this ModelManager mmng)
		{
			return mmng.GetChatMessage();
		}

		private static List<ChatMessage> GetChatMessage(this ModelManager mmng)
		{
			ChatMessageData chatMessageData = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_chatMessage))
			{
				chatMessageData = mmng.GetData<ChatMessageData>(EModelType.Model_chatMessage);
			}
			return chatMessageData.CahtMessageList;
		}
	}
}
