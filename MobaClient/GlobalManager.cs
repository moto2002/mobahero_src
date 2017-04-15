using System;

namespace MobaClient
{
	public class GlobalManager
	{
		public const int CLIENT_NET = 1;

		public const int CLIENT_UNITY = 2;

		public const int CLIENT_CONSOLE = 3;

		public static int ClientType = 3;

		private static GlobalManager globalManager;

		public static int GameServerId;

		public static string masterIpForce
		{
			get;
			set;
		}

		public static GlobalManager Singleton
		{
			get
			{
				if (GlobalManager.globalManager == null)
				{
					GlobalManager.globalManager = new GlobalManager();
				}
				return GlobalManager.globalManager;
			}
		}

		public string ClientChannelId
		{
			get;
			set;
		}
	}
}
