using System;

namespace GUIFramework
{
	public class CloseWindowEventArgs : EventArgs
	{
		public bool IsSuccess
		{
			get;
			private set;
		}

		public string WinName
		{
			get;
			private set;
		}

		public bool IsDestroy
		{
			get;
			private set;
		}

		public bool IsRestart
		{
			get;
			private set;
		}

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
