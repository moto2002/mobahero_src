using System;

namespace TUI
{
	public class TUISprite : TUIWidget
	{
		public UISprite sprite;

		private void Awake()
		{
			if (this.sprite == null)
			{
				this.sprite = base.transform.GetComponent<UISprite>();
			}
		}
	}
}
