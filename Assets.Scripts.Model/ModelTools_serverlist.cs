using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
	public static class ModelTools_serverlist
	{
		public static ServerListModelData Get_serverInfo(this ModelManager mmng)
		{
			ServerListModelData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_serverlist))
			{
				result = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist);
			}
			return result;
		}

		public static List<ServerInfo> Get_ServerList_X(this ModelManager mmng)
		{
			List<ServerInfo> result = new List<ServerInfo>();
			if (mmng != null && mmng.ValidData(EModelType.Model_serverlist))
			{
				result = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist).serverlist;
			}
			return result;
		}

		public static ServerInfo Get_serverInfoByIndex_X(this ModelManager mmng, int index)
		{
			ServerInfo result = null;
			List<ServerInfo> list = mmng.Get_ServerList_X();
			if (list != null && index >= 0 && index < list.Count)
			{
				result = list[index];
			}
			return result;
		}

		public static int Get_currLoginServerIndex_X(this ModelManager mmng)
		{
			int result = 0;
			if (mmng != null && mmng.ValidData(EModelType.Model_serverlist))
			{
				result = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist).currLoginServerIndex;
			}
			return result;
		}

		public static ServerInfo Get_curLoginServerInfo(this ModelManager mmng)
		{
			ServerInfo result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_serverlist))
			{
				ServerListModelData data = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist);
				if (data != null)
				{
					List<ServerInfo> serverlist = data.serverlist;
					if (serverlist != null && data.currLoginServerIndex >= 0 && data.currLoginServerIndex < serverlist.Count)
					{
						result = serverlist[data.currLoginServerIndex];
					}
				}
			}
			return result;
		}

		public static void Set_curLoginServerIndex_X(this ModelManager mmng, int index)
		{
			if (mmng != null && mmng.ValidData(EModelType.Model_serverlist))
			{
				ServerListModelData data = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist);
				if (data != null)
				{
					List<ServerInfo> serverlist = data.serverlist;
					if (serverlist != null && index >= 0 && index < serverlist.Count)
					{
						data.currLoginServerIndex = index;
						PlayerPrefs.SetInt("LastLoginAreaId", data.serverlist[index].areaId);
						PlayerPrefs.Save();
					}
				}
			}
		}

		public static void Get_GamePeerIP_X(this ModelManager mmng)
		{
			ServerListModelData data = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist);
			List<ServerInfo> serverlist = data.serverlist;
			int currLoginServerIndex = data.currLoginServerIndex;
			if (serverlist == null)
			{
				return;
			}
			if (currLoginServerIndex >= 0 && currLoginServerIndex < serverlist.Count)
			{
				MobaPeer mobaPeer = NetWorkHelper.Instance.client.GetMobaPeer(MobaPeerType.C2GateServer, true);
				ConnectionProtocol usedProtocol = mobaPeer.UsedProtocol;
				if (usedProtocol != ConnectionProtocol.Udp)
				{
					if (usedProtocol == ConnectionProtocol.Tcp)
					{
						data.mGamePeerServerName = serverlist[currLoginServerIndex].tcpaddress;
					}
				}
				else
				{
					data.mGamePeerServerName = serverlist[currLoginServerIndex].udpaddress;
				}
				data.mGamePeerAppName = serverlist[currLoginServerIndex].servername;
				data.ServerId = serverlist[currLoginServerIndex].serverid;
			}
		}

		public static string Get_OnlyServerKey_X(this ModelManager mmng)
		{
			string result = string.Empty;
			ServerListModelData serverListModelData = mmng.Get_serverInfo();
			if (serverListModelData != null)
			{
				result = serverListModelData.OnlyServerKey;
			}
			return result;
		}

		public static Dictionary<int, AreaInfo> Get_AreaList_X(this ModelManager mmng)
		{
			Dictionary<int, AreaInfo> result = new Dictionary<int, AreaInfo>();
			if (mmng != null && mmng.ValidData(EModelType.Model_serverlist))
			{
				result = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist).arealist;
			}
			return result;
		}

		public static AreaInfo Get_RecommendArea(this ModelManager mmng)
		{
			AreaInfo result = null;
			string recommendServerId = mmng.Get_serverInfo().RecommendServerId;
			if (string.IsNullOrEmpty(recommendServerId))
			{
				return result;
			}
			Dictionary<int, AreaInfo> dictionary = mmng.Get_AreaList_X();
			if (dictionary != null && dictionary.ContainsKey(Convert.ToInt32(recommendServerId)))
			{
				result = dictionary[Convert.ToInt32(recommendServerId)];
			}
			return result;
		}

		public static AreaInfo Get_LastLoginServer_X(this ModelManager mmng)
		{
			AreaInfo result = null;
			int @int = PlayerPrefs.GetInt("LastLoginAreaId");
			Dictionary<int, AreaInfo> dictionary = mmng.Get_AreaList_X();
			if (dictionary != null && @int >= 0 && dictionary.ContainsKey(@int))
			{
				result = dictionary[@int];
			}
			return result;
		}

		public static AreaInfo Get_currSelectedArea_X(this ModelManager mmng)
		{
			ServerListModelData data = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist);
			ServerInfo serverInfo = data.serverlist[data.currLoginServerIndex];
			return data.arealist[serverInfo.areaId];
		}

		public static int Get_currSelectedAreaId(this ModelManager mmng)
		{
			ServerListModelData data = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist);
			if (data == null)
			{
				return PlayerPrefs.GetInt("LastLoginAreaId");
			}
			ServerInfo serverInfo = data.serverlist[data.currLoginServerIndex];
			return serverInfo.areaId;
		}

		public static void Set_curLoginServerIndexByName_X(this ModelManager mmng, string name)
		{
			if (mmng != null && mmng.ValidData(EModelType.Model_serverlist))
			{
				ServerListModelData data = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist);
				if (data != null)
				{
					Dictionary<int, AreaInfo> arealist = data.arealist;
					List<ServerInfo> serverlist = data.serverlist;
					if (arealist != null)
					{
						string b = string.Empty;
						foreach (KeyValuePair<int, AreaInfo> current in arealist)
						{
							if (current.Value.areaName == name)
							{
								b = current.Value.selectServer.serverid;
							}
						}
						int currLoginServerIndex = 0;
						for (int i = 0; i < serverlist.Count; i++)
						{
							if (serverlist[i].serverid == b)
							{
								currLoginServerIndex = i;
							}
						}
						data.currLoginServerIndex = currLoginServerIndex;
					}
				}
			}
		}

		public static string Get_currSelectedServerId(this ModelManager mmng)
		{
			if (mmng != null && mmng.ValidData(EModelType.Model_serverlist))
			{
				ServerListModelData data = mmng.GetData<ServerListModelData>(EModelType.Model_serverlist);
				if (data != null && data.currLoginServerIndex < data.serverlist.Count)
				{
					return data.serverlist[data.currLoginServerIndex].serverid;
				}
			}
			return string.Empty;
		}
	}
}
