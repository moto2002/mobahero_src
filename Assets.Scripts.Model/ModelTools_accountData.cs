using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Model
{
	public static class ModelTools_accountData
	{
		public static AccountData Get_accountData_X(this ModelManager mmng)
		{
			return mmng.Get_accountData().accountData;
		}

		public static List<string[]> Get_LoginList(this ModelManager mmng)
		{
			return mmng.Get_accountData().loginDataList;
		}

		public static void Set_accountData_X(this ModelManager mmng, AccountData newData)
		{
			mmng.Set_accountData(newData);
		}

		public static T Get_accountData_filed_X<T>(this ModelManager mmng, string propertyName)
		{
			T result = default(T);
			AccountData accountData = mmng.Get_accountData_X();
			if (accountData != null)
			{
				Type typeFromHandle = typeof(AccountData);
				PropertyInfo property = typeFromHandle.GetProperty(propertyName);
				if (property != null)
				{
					result = (T)((object)property.GetGetMethod().Invoke(accountData, new object[0]));
				}
			}
			return result;
		}

		private static AccountModelData Get_accountData(this ModelManager mmng)
		{
			AccountModelData result = new AccountModelData();
			if (mmng != null && mmng.ValidData(EModelType.Model_accountData))
			{
				result = mmng.GetData<AccountModelData>(EModelType.Model_accountData);
			}
			return result;
		}

		private static void Set_accountData(this ModelManager mmng, AccountData newData)
		{
			if (newData != null && mmng != null && mmng.ValidData(EModelType.Model_accountData))
			{
				mmng.InvokeModelMem(EModelType.Model_accountData, "SetData", new object[]
				{
					newData
				});
			}
		}

		public static void DeleteAccountKey(this ModelManager mmng, string accountName)
		{
			for (int i = 1; i <= 3; i++)
			{
				string str = (i != 1) ? i.ToString() : string.Empty;
				string key = "UserName" + str;
				if (PlayerPrefs.HasKey(key) && accountName == PlayerPrefs.GetString(key))
				{
					PlayerPrefs.DeleteKey(key);
					PlayerPrefs.DeleteKey("PassWord" + str);
					return;
				}
			}
		}
	}
}
