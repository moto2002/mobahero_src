using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class LoginNewsItem_notice : MonoBehaviour, ILoginNewsItem
	{
		[SerializeField]
		private UILabel lb_content;

		[SerializeField]
		private BoxCollider box;

		[SerializeField]
		private BoxCollider bar;

		[SerializeField]
		private LoginDownLoadNotice downLoader;

		public DownLoadAPKNewsItem Info
		{
			get;
			set;
		}

		public GameObject Obj
		{
			get
			{
				return base.gameObject;
			}
		}

		public Action<GameObject, bool> OnHandle
		{
			get;
			set;
		}

		private void Awake()
		{
			UIEventListener.Get(this.box.gameObject).onPress = new UIEventListener.BoolDelegate(this.OnPress_item);
			UIEventListener.Get(this.bar.gameObject).onPress = new UIEventListener.BoolDelegate(this.OnPress_item);
		}

		public void CheckResource()
		{
			string[] array = this.Info.url.Split(new char[]
			{
				'/'
			});
			string texName = array[array.Length - 1];
			this.downLoader.texName = texName;
			this.downLoader.url = this.Info.url;
			this.downLoader.isFromLocal = false;
		}

		private void OnPress_item(GameObject go, bool state)
		{
			if (this.OnHandle != null)
			{
				this.OnHandle(go, state);
			}
		}
	}
}
