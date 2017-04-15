using System;

namespace TUI
{
	public class TUIInput : TUIWidget
	{
		public UIInput input;

		private void Awake()
		{
			if (this.input == null)
			{
				this.input = base.GetComponent<UIInput>();
			}
		}
	}
}
