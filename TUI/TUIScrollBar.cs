using System;

namespace TUI
{
	public class TUIScrollBar : TUIWidget
	{
		public UIScrollBar scrollBar;

		private void Awake()
		{
			if (this.scrollBar == null)
			{
				this.scrollBar = base.GetComponent<UIScrollBar>();
			}
		}
	}
}
