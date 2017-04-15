using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class LoginNewsItem_texture : MonoBehaviour, ILoginNewsItem
	{
		[SerializeField]
		private UITexture texture;

		[SerializeField]
		private LoginDwonLoadTexture downLoader;

		public DownLoadAPKNewsItem Info
		{
			get;
			set;
		}

		public Action<GameObject, bool> OnHandle
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

		private void Awake()
		{
			UIEventListener.Get(this.texture.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickT);
			UIEventListener.Get(this.texture.gameObject).onPress = new UIEventListener.BoolDelegate(this.OnpRressT);
		}

		private void OnClickT(GameObject obj)
		{
			UniWebViewFacade.Instance.OpenUrl(this.Info.param);
		}

		private void OnpRressT(GameObject go, bool state)
		{
			if (this.OnHandle != null)
			{
				this.OnHandle(go, state);
			}
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
	}
}
