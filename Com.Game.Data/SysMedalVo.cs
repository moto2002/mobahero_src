using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysMedalVo
	{
		public string unikey;

		public int MedalID;

		public string MedalName;

		public string MedalDescribe;

		public int MedalCondition;

		public int MedalProperty;

		public int IsHidden;

		public int MedalIcon;

		public string IsPermanent;
	}
}
