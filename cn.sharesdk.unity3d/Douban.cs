using System;

namespace cn.sharesdk.unity3d
{
	[Serializable]
	public class Douban : DevInfo
	{
		public const int type = 5;

		public string SortId = "14";

		public string ApiKey = "02e2cbe5ca06de5908a863b15e149b0b";

		public string Secret = "9f1e7b4f71304f2f";

		public string RedirectUri = "http://www.sharesdk.cn";
	}
}
