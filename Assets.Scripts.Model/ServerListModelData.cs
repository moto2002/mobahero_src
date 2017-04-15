using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class ServerListModelData
	{
		private List<ServerInfo> mServerlist = new List<ServerInfo>();

		public Dictionary<int, AreaInfo> arealist = new Dictionary<int, AreaInfo>();

		public int currLoginServerIndex;

		public string mGamePeerServerName = string.Empty;

		public string mGamePeerAppName = string.Empty;

		public string OnlyServerKey = string.Empty;

		public string ServerId = string.Empty;

		public string RecommendServerId = string.Empty;

		public string GameId = string.Empty;

		public string SessionId = string.Empty;

		public string LobbyId = string.Empty;

		public string m_GatePeeradressIP = string.Empty;

		public int m_GatePeerPort;

		public string m_TokenKey = string.Empty;

		public List<ServerInfo> serverlist
		{
			get
			{
				return this.mServerlist;
			}
			set
			{
				this.mServerlist = value;
				this.GetAreaList();
			}
		}

		private void GetAreaList()
		{
			this.arealist.Clear();
			if (this.mServerlist == null)
			{
				return;
			}
			foreach (ServerInfo current in this.mServerlist)
			{
				if (current == null)
				{
					break;
				}
				if (!this.arealist.ContainsKey(current.areaId))
				{
					this.arealist.Add(current.areaId, new AreaInfo());
				}
				this.arealist[current.areaId].AddServer(current);
			}
		}
	}
}
