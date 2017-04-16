using System;

namespace GUIFramework
{
    /// <summary>
    /// 关闭窗口事件属性类
    /// </summary>
	public class CloseWindowEventArgs : EventArgs
	{
        /// <summary>
        /// 是否成功
        /// </summary>
		public bool IsSuccess
		{
			get;
			private set;
		}
        /// <summary>
        /// 窗口名称
        /// </summary>
		public string WinName
		{
			get;
			private set;
		}
        /// <summary>
        /// 是否销毁窗口
        /// </summary>
		public bool IsDestroy
		{
			get;
			private set;
		}
        /// <summary>
        /// 是否重新开始
        /// </summary>
		public bool IsRestart
		{
			get;
			private set;
		}
        /// <summary>
        /// 事件相关的TUIWindow窗口
        /// </summary>
		public TUIWindow UiWindow
		{
			get;
			private set;
		}

		public CloseWindowEventArgs(bool success, string name, bool isDestroy, bool isRestart, TUIWindow win)
		{
			this.IsSuccess = success;
			this.WinName = name;
			this.IsDestroy = isDestroy;
			this.IsRestart = isRestart;
			this.UiWindow = win;
		}
	}
}
