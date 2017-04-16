using System;

namespace GUIFramework
{
    /// <summary>
    /// 打开窗口事件属性
    /// </summary>
	public class OpenWindowEventArgs : EventArgs
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
        /// 相关的TUIWindow
        /// </summary>
		public TUIWindow UiWindow
		{
			get;
			private set;
		}

		public OpenWindowEventArgs(bool success, string name, TUIWindow window)
		{
			this.IsSuccess = success;
			this.WinName = name;
			this.UiWindow = window;
		}
	}
}
