using System;

namespace GUIFramework
{
    /// <summary>
    /// 窗口类型
    /// </summary>
	public enum WindowType
	{
		Background = 1,
		Normal = 10,
        /// <summary>
        /// 半固定
        /// </summary>
		SemiFixed = 50,
        /// <summary>
        /// 固定
        /// </summary>
		Fixed = 100,
		Popup = 120,
		Hint = 150,
		Highest = 1500
	}
}
