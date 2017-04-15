using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class NetInfo
	{
		public string getMasterIPUrl;

		public string masterIp;

		public string masterForceIp;

		public photontype masterType;

		public string appName;

		public string defaultIP;

		public int masterPort;

		public Dictionary<string, string> dicMasterIp;

		public NetInfo()
		{
			this.appName = "MobaServer.LoginServer";
			this.getMasterIPUrl = "http://cdn.xiaomeng.cc/FILE/MasterConfig/config.json";
			this.defaultIP = "mobaapp.xiaomeng.cc";
			this.masterPort = 8080;
			this.masterType = photontype.Net;
			this.dicMasterIp = new Dictionary<string, string>();
		}

		public void UpdateMasterIp()
		{
			string empty = string.Empty;
			if (!string.IsNullOrEmpty(this.masterForceIp))
			{
				this.masterIp = this.masterForceIp;
			}
			else if (this.dicMasterIp.TryGetValue(this.masterType.ToString(), out empty))
			{
				this.masterIp = empty;
			}
			else
			{
				this.masterIp = this.defaultIP;
			}
		}
	}
}
