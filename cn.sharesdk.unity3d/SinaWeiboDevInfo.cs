using System;

namespace cn.sharesdk.unity3d
{
	[Serializable]
	public class SinaWeiboDevInfo : DevInfo
	{
		public const int type = 1;

		public string SortId = "4";

		public string AppKey = "48558435";

		public string AppSecret = "5b2c254ae582a9fb61b95cc925dbf0d4";

		public string RedirectUrl = "http://mobahero.com";

		public bool ShareByAppClient = true;
	}
}
