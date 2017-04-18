using System;

namespace GUIFramework
{
    /// <summary>
    /// 窗口资源配置类
    /// </summary>
	public class WinResurceCfg
	{
        /// <summary>
        /// 是否AssetBundle
        /// </summary>
		public bool IsAssetbundle
		{
			get;
			private set;
		}

        /// <summary>
        /// 是否从配置加载
        /// </summary>
		public bool IsLoadFromConfig
		{
			get;
			private set;
		}

        /// <summary>
        /// 加载链接或路径????
        /// </summary>
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
