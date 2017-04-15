using System;

namespace TUI
{
	public class TUIPopupList : TUIWidget
	{
		public UIPopupList popupList;

		private void Awake()
		{
			if (this.popupList == null)
			{
				this.popupList = base.GetComponent<UIPopupList>();
			}
		}
	}
}
