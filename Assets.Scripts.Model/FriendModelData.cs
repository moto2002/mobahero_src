using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class FriendModelData
	{
		public List<FriendData> friendDataList;

		public List<NotificationData> notificationDataList;

		public FriendData userInfo;

		public bool getFriendDataListOk;

		public bool applyAddFriendOK;

		public bool modifyFriendStatusOK;

		public bool getUserInfoBySummIdOK;

		public bool getFriendMessagesOK;

		public FriendState friendState;

		public List<FriendData> radarFriendList;
	}
}
