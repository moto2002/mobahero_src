using System;

namespace TUI
{
	public class TUISlider : TUIWidget
	{
		public UISlider slider;

		private void Awake()
		{
			if (this.slider == null)
			{
				this.slider = base.GetComponent<UISlider>();
			}
		}
	}
}
