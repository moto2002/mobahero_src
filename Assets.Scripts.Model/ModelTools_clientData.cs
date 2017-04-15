using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_clientData
	{
		public static ClientData Get_ClientData_X(this ModelManager mmng)
		{
			ClientData clientData = mmng.GetClientData();
			return clientData ?? new ClientData();
		}

		private static ClientData GetClientData(this ModelManager mmng)
		{
			ClientData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_clientData))
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null)
				{
					result = data.clientData;
				}
			}
			return result;
		}

		public static string Get_appNetworkUrlProperty(this ModelManager mmng)
		{
			string result = string.Empty;
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null)
				{
					result = data.appNetworkUrl_property;
				}
			}
			return result;
		}

		public static void Set_appNetworkUrlProperty(this ModelManager mmng, string appUrl)
		{
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null)
				{
					data.appNetworkUrl_property = appUrl;
				}
			}
		}

		public static string Get_IPProperty(this ModelManager mmng)
		{
			string result = string.Empty;
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null)
				{
					result = data.IP_property;
				}
			}
			return result;
		}

		public static bool Get_needDownloadAPK(this ModelManager mmng)
		{
			bool result = false;
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null)
				{
					result = data.downLoadAPK;
				}
			}
			return result;
		}

		public static string Get_updateContentUrl(this ModelManager mmng)
		{
			string result = string.Empty;
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null && data.clientData != null)
				{
					result = data.clientData.updateContentUrl;
				}
			}
			return result;
		}

		public static string Get_AppUpgradeUrl(this ModelManager mmng)
		{
			string result = string.Empty;
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null && data.clientData != null)
				{
					result = data.clientData.AppUpgradeUrl;
				}
			}
			return result;
		}

		public static string Get_appVersionProperty(this ModelManager mmng)
		{
			string result = string.Empty;
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null)
				{
					result = data.appVersion_property;
				}
			}
			return result;
		}

		public static void Set_appVersionProperty(this ModelManager mmng, string appVersion)
		{
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null)
				{
					data.appVersion_property = appVersion;
				}
			}
		}

		public static bool Get_IsInWhiteList(this ModelManager mmng)
		{
			if (mmng != null)
			{
				ClientModelData data = mmng.GetData<ClientModelData>(EModelType.Model_clientData);
				if (data != null && data.clientData != null)
				{
					return data.clientData.IsInWhiteList;
				}
			}
			return false;
		}
	}
}
