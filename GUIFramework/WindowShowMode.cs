using System;

namespace GUIFramework
{
    /// <summary>
    /// 窗口打开模式类
    /// </summary>
	public enum WindowShowMode
	{
        /// <summary>
        /// 普通直接打开
        /// </summary>
		DoNothing,
        /// <summary>
        /// 打开时隐藏其他窗口
        /// </summary>
		HideOther,
        /// <summary>
        /// 打开窗口需要返回支持
        /// </summary>
		NeedReturn,
        /// <summary>
        /// 打开窗口不需要返回
        /// </summary>
		UnneedReturn,
        /// <summary>
        /// 隐藏所有
        /// </summary>
		HideAll
	}
}
