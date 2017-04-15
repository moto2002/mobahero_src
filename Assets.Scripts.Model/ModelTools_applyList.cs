using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Model
{
	public static class ModelTools_applyList
	{
		private static T Get_applyList_field<T>(this ModelManager mmng, string propertyName)
		{
			T result = default(T);
			List<FriendData> list = mmng.Get_applyList_X();
			if (list != null)
			{
				Type typeFromHandle = typeof(List<FriendData>);
				PropertyInfo property = typeFromHandle.GetProperty(propertyName);
				if (property != null)
				{
					result = (T)((object)property.GetGetMethod().Invoke(list, new object[0]));
				}
			}
			return result;
		}

		public static List<FriendData> Get_applyList_X(this ModelManager mmng)
		{
			List<FriendData> list = mmng.GetApplyList();
			if (list == null)
			{
				list = new List<FriendData>();
			}
			return list;
		}

		private static List<FriendData> GetApplyList(this ModelManager mmng)
		{
			List<FriendData> result = new List<FriendData>();
			if (mmng != null && mmng.ValidData(EModelType.Model_applyList))
			{
				result = mmng.GetData<List<FriendData>>(EModelType.Model_applyList);
			}
			return result;
		}
	}
}
