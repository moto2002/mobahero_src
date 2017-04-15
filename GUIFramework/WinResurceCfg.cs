using System;

namespace GUIFramework
{
	public class WinResurceCfg
	{
		public bool IsAssetbundle
		{
			get;
			private set;
		}

		public bool IsLoadFromConfig
		{
			get;
			private set;
		}

		public string Url
		{
			get;
			private set;
		}

		public WinResurceCfg(bool isAssetBundle, bool isLoadFromcfg, string url)
		{
			this.IsAssetbundle = isAssetBundle;
			this.IsLoadFromConfig = isLoadFromcfg;
			this.Url = url;
		}
	}
}
