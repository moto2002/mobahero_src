using System;
using System.Collections.Generic;

namespace TUI
{
	public class TUICheckBox : TUIWidget
	{
		public UIToggle toggle;

		public UILabel label;

		public List<EventDelegate> onChange
		{
			get
			{
				return this.toggle.onChange;
			}
		}

		public bool value
		{
			get
			{
				return this.toggle.value;
			}
			set
			{
				this.toggle.value = value;
			}
		}

		public bool enable
		{
			get
			{
				return base.enabled;
			}
			set
			{
				base.enabled = value;
			}
		}

		private void Awake()
		{
			if (this.toggle == null)
			{
				base.GetComponent<UIToggle>();
			}
		}
	}
}
