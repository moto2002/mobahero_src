using Com.Game.Utils;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_Net
	{
		private static NetInfo Get_NetInfo(this ModelManager mmng)
		{
			NetInfo netInfo = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_Net))
			{
				netInfo = mmng.GetData<NetInfo>(EModelType.Model_Net);
			}
			if (netInfo == null)
			{
				ClientLogger.Error("NetInfo=null，初始化有问题");
			}
			return netInfo;
		}

		public static string Get_masterIpUrl(this ModelManager mmng)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			return netInfo.getMasterIPUrl;
		}

		public static string Get_masterIp(this ModelManager mmng)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			return netInfo.masterIp;
		}

		public static string Get_masterForceIp(this ModelManager mmng)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			return netInfo.masterForceIp;
		}

		public static string Get_appName(this ModelManager mmng)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			return netInfo.appName;
		}

		public static photontype Get_masterType(this ModelManager mmng)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			return netInfo.masterType;
		}

		public static string Get_masterDefaultIp(this ModelManager mmng)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			return netInfo.defaultIP;
		}

		public static int Get_masterPort(this ModelManager mmng)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			return netInfo.masterPort;
		}

		public static void Set_masterForceIp(this ModelManager mmng, string forceIP)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			netInfo.masterForceIp = forceIP;
			netInfo.UpdateMasterIp();
		}

		public static void Set_masterIpList(this ModelManager mmng, Dictionary<string, string> dic)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			if (dic != null)
			{
				netInfo.dicMasterIp = dic;
			}
			else
			{
				netInfo.dicMasterIp = new Dictionary<string, string>();
			}
			netInfo.UpdateMasterIp();
		}

		private static void Set_masterIp(this ModelManager mmng, string ip)
		{
			NetInfo netInfo = mmng.Get_NetInfo();
			netInfo.masterIp = ip;
		}
	}
}
