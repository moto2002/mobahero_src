using System;
using UnityEngine;

namespace GUIFramework
{
    /// <summary>
    /// 窗口打开显示配置类
    /// </summary>
	[Serializable]
	public class WinShowCfg
	{
        /// <summary>
        /// 窗口打开模式
        /// </summary>
		[Tooltip("窗口打开模式")]
		public WindowShowMode ShowMode;

        /// <summary>
        /// 是否清除返回窗口序列
        /// </summary>
		[Tooltip("是否清除返回窗口序列")]
		public bool IsClearReturnSeq;

        /// <summary>
        /// 是否加入到返回队列
        /// </summary>
		[Tooltip("是否加入到返回队列")]
		public bool IsAddToReturnSeq;
	}
}
