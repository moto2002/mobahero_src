using System;

namespace cn.sharesdk.unity3d
{
	[Serializable]
	public class TencentWeiboDevInfo : DevInfo
	{
		public const int type = 2;

		public string SortId = "3";

		public string AppKey = "801307650";

		public string AppSecret = "ae36f4ee3946e1cbb98d6965b0b2ff5c";

		public string RedirectUri = "http://sharesdk.cn";
	}
}
