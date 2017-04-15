using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_Friend
	{
		public static List<FriendData> Get_FriendDataList_X(this ModelManager mmng)
		{
			List<FriendData> list = null;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				list = mmng.GetFriendModelData().friendDataList;
			}
			return list ?? new List<FriendData>();
		}

		private static FriendModelData GetFriendModelData(this ModelManager mmng)
		{
			FriendModelData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_Friend))
			{
				result = mmng.GetData<FriendModelData>(EModelType.Model_Friend);
			}
			return result;
		}

		public static List<NotificationData> GetNotificationDataList(this ModelManager mmng)
		{
			List<NotificationData> result = null;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				result = mmng.GetFriendModelData().notificationDataList;
			}
			return result;
		}

		public static FriendData GetFriendUserInfo(this ModelManager mmng)
		{
			FriendData result = null;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().userInfo != null)
			{
				result = mmng.GetFriendModelData().userInfo;
			}
			return result;
		}

		public static FriendState GetFriendState(this ModelManager mmng)
		{
			FriendState result = null;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				result = mmng.GetFriendModelData().friendState;
			}
			return result;
		}

		public static bool CheckGetFriendDataListOk(this ModelManager mmng)
		{
			bool result = false;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				result = mmng.GetFriendModelData().getFriendDataListOk;
			}
			return result;
		}

		public static bool CheckApplyAddFriendOK(this ModelManager mmng)
		{
			bool result = false;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				result = mmng.GetFriendModelData().applyAddFriendOK;
			}
			return result;
		}

		public static bool CheckModifyFriendStatusOK(this ModelManager mmng)
		{
			bool result = false;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				result = mmng.GetFriendModelData().modifyFriendStatusOK;
			}
			return result;
		}

		public static bool CheckGetUserInfoBySummIdOK(this ModelManager mmng)
		{
			bool result = false;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				result = mmng.GetFriendModelData().getUserInfoBySummIdOK;
			}
			return result;
		}

		public static bool CheckGetFriendMessagesOK(this ModelManager mmng)
		{
			bool result = false;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				result = mmng.GetFriendModelData().getFriendMessagesOK;
			}
			return result;
		}

		public static List<FriendData> Get_RadarFriendList_X(this ModelManager mmng)
		{
			List<FriendData> result = null;
			if (mmng.GetFriendModelData() != null && mmng.GetFriendModelData().friendDataList != null)
			{
				result = mmng.GetFriendModelData().radarFriendList;
			}
			return result;
		}
	}
}
