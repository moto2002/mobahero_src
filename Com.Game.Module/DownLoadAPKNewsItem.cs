using System;

namespace Com.Game.Module
{
	[Serializable]
	public class DownLoadAPKNewsItem
	{
		public int id;

		public string url;

		public int sequence;

		public string param;

		public string title;

		public int type;

		public string unikey;

		public static int Compare(DownLoadAPKNewsItem x, DownLoadAPKNewsItem y)
		{
			if (x == null && y == null)
			{
				return 0;
			}
			if (x == null && y != null)
			{
				return -1;
			}
			if (x != null && y == null)
			{
				return 1;
			}
			if (x.sequence < y.sequence)
			{
				return -1;
			}
			if (x.sequence > y.sequence)
			{
				return 1;
			}
			return 0;
		}
	}
}
