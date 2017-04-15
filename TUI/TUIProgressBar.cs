using System;

namespace TUI
{
	public class TUIProgressBar : TUIWidget
	{
		public UIProgressBar progressBar;

		private void Awake()
		{
			if (this.progressBar == null)
			{
				this.progressBar = base.GetComponent<UIProgressBar>();
			}
		}
	}
}
