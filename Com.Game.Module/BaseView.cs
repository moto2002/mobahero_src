using GUIFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class BaseView<T> : Singleton<T>, IView where T : new()
	{
		private GameObject mGameObject;

		protected object intiData;

		public OpenViewGuideDelegate AfterOpenGuideDelegate;

		protected List<string> prefabCacheKeys = new List<string>();

		private GameObject animationRoot;

		public bool IsForceReset = true;

		public string WindowTitle
		{
			get;
			set;
		}

		public WinResurceCfg WinResCfg
		{
			get;
			set;
		}

		public TUIWindow uiWindow
		{
			get;
			set;
		}

		public WindowID WinId
		{
			get;
			set;
		}

		public Transform transform
		{
			get
			{
				if (this.uiWindow == null)
				{
					return null;
				}
				return this.uiWindow.trans;
			}
		}

		public bool IsOpen
		{
			get
			{
				return null != this.gameObject && this.gameObject.activeInHierarchy;
			}
		}

		public GameObject gameObject
		{
			get
			{
				return (!this.uiWindow) ? null : this.uiWindow.gameObj;
			}
			set
			{
				this.mGameObject = value;
			}
		}

		public GameObject AnimationRoot
		{
			get
			{
				return this.animationRoot;
			}
			set
			{
				this.animationRoot = value;
			}
		}

		public bool IsOpened
		{
			get;
			set;
		}

		public virtual void Init()
		{
		}

		public virtual void HandleAfterOpenView()
		{
		}

		public virtual void HandleBeforeCloseView()
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.signView_close, null, false);
		}

		public virtual void RegisterUpdateHandler()
		{
		}

		public virtual void CancelUpdateHandler()
		{
		}

		public virtual void RefreshUI()
		{
		}

		public virtual void OnRestart()
		{
		}

		public virtual void Destroy()
		{
			this.UnloadPrefabCache();
			this.uiWindow = null;
		}

		public virtual void DataUpdated(object data)
		{
			this.intiData = data;
		}

		protected GameObject LoadPrefabCache(string name)
		{
			GameObject result = ResourceManager.Load<GameObject>(name, true, this.WinResCfg.IsAssetbundle, null, 0, false);
			if (!this.prefabCacheKeys.Contains(name))
			{
				this.prefabCacheKeys.Add(name);
			}
			return result;
		}

		protected void UnloadPrefabCache()
		{
			if (this.prefabCacheKeys != null)
			{
				string[] names = this.prefabCacheKeys.ToArray();
				ResourceManager.UnLoadBundle(names, this.WinResCfg.IsAssetbundle);
				this.prefabCacheKeys.Clear();
			}
		}

		public virtual bool DoReturnLogic()
		{
			return true;
		}

		public virtual void Initialize()
		{
			if (this.IsForceReset)
			{
				this.BindObject();
				this.IsForceReset = false;
			}
			this.RegisterListener();
		}

		public virtual void HandleHideEvent()
		{
			this.UnRegisterListener();
		}

		public virtual void HandleDestroyEvent()
		{
			this.HandleHideEvent();
			this.IsForceReset = true;
		}

		protected virtual void BindObject()
		{
		}

		protected virtual void RegisterListener()
		{
		}

		protected virtual void UnRegisterListener()
		{
		}
	}
}
