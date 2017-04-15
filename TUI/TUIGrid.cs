using System;

namespace TUI
{
	public class TUIGrid : TUIWidget
	{
		public UIGrid grid;

		private void Awake()
		{
			if (this.grid == null)
			{
				this.grid = base.GetComponent<UIGrid>();
			}
		}
	}
}
