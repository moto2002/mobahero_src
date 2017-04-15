using System;

namespace TUI
{
	public class TUIPanel : TUIWidget
	{
		public UIPanel panel;

		private void Awake()
		{
			if (this.panel == null)
			{
				this.panel = base.GetComponent<UIPanel>();
			}
		}
	}
}
