using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class AreaInfo
	{
		public string areaName;

		public string areaImage;

		public byte areaState;

		private int selectedIndex;

		public List<ServerInfo> serverlist;

		public ServerInfo selectServer
		{
			get
			{
				if (this.serverlist.Count >= this.selectedIndex)
				{
					return this.serverlist[this.selectedIndex];
				}
				return null;
			}
		}

		public AreaInfo()
		{
			this.selectedIndex = -1;
			this.areaName = "NullArea";
			this.areaState = 0;
			this.areaImage = string.Empty;
			this.serverlist = new List<ServerInfo>();
		}

		public void AddServer(ServerInfo item)
		{
			this.serverlist.Add(item);
			if (this.serverlist.Count == 1)
			{
				if (ModelManager.Instance.Get_IsInWhiteList() && GlobalSettings.Instance.versionConfig.appVersion != item.appVer)
				{
					this.areaName = item.servername + "(" + item.appVer + ")";
				}
				else
				{
					this.areaName = item.servername;
				}
				this.areaImage = item.serverImage;
				this.areaState = item.serverState;
				this.selectedIndex = 0;
			}
			else if ((this.areaState == 0 && item.serverState > 0) || (this.areaState > 0 && item.serverState > 0 && item.serverState < this.areaState))
			{
				this.areaState = item.serverState;
				this.selectedIndex = this.serverlist.Count - 1;
			}
		}
	}
}
