using System;

namespace TUI
{
	public class TUILabel : TUIWidget
	{
		public UILabel label;

		public string text
		{
			get
			{
				return this.label.text;
			}
			set
			{
				this.label.text = value;
			}
		}

		private void Awake()
		{
			if (this.label == null)
			{
				this.label = base.GetComponent<UILabel>();
			}
		}
	}
}
