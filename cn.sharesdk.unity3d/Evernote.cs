using System;

namespace cn.sharesdk.unity3d
{
	[Serializable]
	public class Evernote : DevInfo
	{
		public enum HostType
		{
			sandbox = 1,
			china,
			product
		}

		public const int type = 12;

		public string SortId = "17";

		public string ConsumerKey = "sharesdk-7807";

		public string ConsumerSecret = "d05bf86993836004";

		public bool ShareByAppClient;
	}
}
