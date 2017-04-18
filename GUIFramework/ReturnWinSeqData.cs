using System;
using System.Collections.Generic;

namespace GUIFramework
{
    /// <summary>
    /// 窗口退出请求数据类
    /// </summary>
	public class ReturnWinSeqData
	{
        /// <summary>
        /// 隐藏的目标窗口
        /// </summary>
		public TUIWindow hideTargetWindow;
        /// <summary>
        /// 退出窗口，显示的目标窗口列表
        /// </summary>
		public List<string> lstReturnShowTargets;
	}
}
