using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_blacklist
	{
		public static List<FriendData> Get_blackList_X(this ModelManager mmng)
		{
			List<FriendData> list = new List<FriendData>();
			list = mmng.GetBlackList();
			if (list == null)
			{
				list = new List<FriendData>();
			}
			return list;
		}

		private static List<FriendData> GetBlackList(this ModelManager mmng)
		{
			List<FriendData> result = new List<FriendData>();
			if (mmng != null && mmng.ValidData(EModelType.Model_blackList))
			{
				result = mmng.GetData<List<FriendData>>(EModelType.Model_blackList);
			}
			return result;
		}
	}
}
