using System;

namespace Assets.Scripts.Character.Control
{
	public class ManualControlSignalMng
	{
		private ManualController controller;

		private bool isHandling;

		public string debuginf = " ";

		public string handleStr = string.Empty;

		public bool Handling
		{
			get
			{
				return this.isHandling;
			}
		}

		public ManualControlSignalMng(ManualController c)
		{
			this.controller = c;
		}

		public void Init()
		{
			this.isHandling = false;
			this.handleStr = string.Empty;
		}

		public void SetHandling()
		{
			this.isHandling = true;
		}

		public string GetHandling()
		{
			return this.handleStr;
		}

		public void SetHandling(string debugInfo)
		{
			this.handleStr = debugInfo;
			this.debuginf = debugInfo;
			this.isHandling = true;
			this.controller.ShowHandlerDebug(debugInfo);
		}
	}
}
