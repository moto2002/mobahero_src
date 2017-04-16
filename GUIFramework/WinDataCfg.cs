using System;
using UnityEngine;

namespace GUIFramework
{
    /// <summary>
    /// 窗口数据配置类
    /// </summary>
	[Serializable]
	public class WinDataCfg
	{
        /// <summary>
        /// 是否为常驻UI（即使是全屏，也不清除）
        /// </summary>
		[Tooltip("是否为常驻UI（即使是全屏，也不清除）")]
		public bool IsDestroy = true;

        /// <summary>
        /// 关闭是否需要延时
        /// </summary>
		[Tooltip("关闭是否需要延时")]
		public bool IsDelayClose;

        /// <summary>
        /// 打开的标题是否需要延时
        /// </summary>
		[Tooltip("打开的标题是否需要延时")]
		public bool IsDelayShowBar;

        /// <summary>
        /// 是否起始窗口
        /// </summary>
		[Tooltip("是否起始窗口")]
		public bool IsOnset;

        /// <summary>
        /// 窗口类型
        /// </summary>
		[Tooltip("窗口类型")]
		public WindowType WinType = WindowType.Normal;
	}
}
