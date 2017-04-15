using System;

namespace GUIFramework
{
	public class OpenWindowEventArgs : EventArgs
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
