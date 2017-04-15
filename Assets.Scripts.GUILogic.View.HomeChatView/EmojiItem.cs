using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.HomeChatView
{
	public class EmojiItem : MonoBehaviour
	{
		[SerializeField]
		private UISprite sprite;

		private UIDragScrollView udsv;

		public Callback<GameObject, EmojiItem> ClickCallBack;

		private int num;

		private string emojiName;

		private int Num
		{
			get
			{
				return this.num;
			}
		}

		public string EmojiName
		{
			get
			{
				return "#e" + this.emojiName;
			}
		}

		public UISprite Sprite
		{
			get
			{
				return this.sprite;
			}
		}

		private void Awake()
		{
			this.udsv = this.sprite.GetComponent<UIDragScrollView>();
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init(string name)
		{
			this.emojiName = name;
			base.gameObject.name = name;
			this.num = -int.Parse(name);
			this.sprite.spriteName = name;
			UIEventListener.Get(this.sprite.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEmoji);
			this.udsv.scrollView = base.transform.parent.parent.GetComponent<UIScrollView>();
		}

		public void ClickEmoji(GameObject obj)
		{
			if (this.ClickCallBack != null)
			{
				this.ClickCallBack(obj, this);
			}
		}
	}
}
