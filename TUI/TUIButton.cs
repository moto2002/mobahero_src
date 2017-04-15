using System;

namespace TUI
{
	public class TUIButton : TUIWidget
	{
		public UIButton button;

		public UILabel label;

		public UISprite backSprite;

		public bool isEnabled
		{
			get
			{
				return this.button.isEnabled;
			}
			set
			{
				this.button.isEnabled = value;
			}
		}

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

		public int fontsize
		{
			get
			{
				return this.label.fontSize;
			}
			set
			{
				this.label.fontSize = value;
			}
		}

		public string background
		{
			get
			{
				return this.backSprite.spriteName;
			}
			set
			{
				this.backSprite.spriteName = value;
			}
		}

		private void Awake()
		{
			if (this.button == null)
			{
				base.GetComponent<UIButton>();
			}
			if (this.label == null)
			{
				base.GetComponentInChildren<UILabel>();
			}
			if (this.backSprite == null)
			{
				base.GetComponentInParent<UISprite>();
			}
		}
	}
}
