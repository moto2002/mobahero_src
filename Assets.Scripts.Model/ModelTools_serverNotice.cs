using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_serverNotice
	{
		private static ServerNoticeModelData get_serverNoticeModelData(this ModelManager mmng)
		{
			ServerNoticeModelData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_serverNotice))
			{
				result = mmng.GetData<ServerNoticeModelData>(EModelType.Model_serverNotice);
			}
			return result;
		}

		public static NotificationData Get_GameNotification(this ModelManager mmng)
		{
			ServerNoticeModelData serverNoticeModelData = mmng.get_serverNoticeModelData();
			return (serverNoticeModelData.gmMsgQueue != null && serverNoticeModelData.gmMsgQueue.Count != 0) ? serverNoticeModelData.gmMsgQueue.Dequeue() : null;
		}

		public static string Get_OtherGameNotification(this ModelManager mmng)
		{
			ServerNoticeModelData serverNoticeModelData = mmng.get_serverNoticeModelData();
			return (serverNoticeModelData.otherMsgQueue != null && serverNoticeModelData.otherMsgQueue.Count != 0) ? serverNoticeModelData.otherMsgQueue.Dequeue() : string.Empty;
		}
	}
}
