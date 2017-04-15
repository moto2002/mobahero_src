using System;

namespace TUI
{
	public class TUITexture : TUIWidget
	{
		public UITexture texture;

		private void Awake()
		{
			if (this.texture == null)
			{
				this.texture = base.GetComponent<UITexture>();
			}
		}
	}
}
