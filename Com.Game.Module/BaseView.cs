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
        /// <summary>
        /// 窗口标题
        /// </summary>
		public string WindowTitle
		{
			get;
			set;
		}
        /// <summary>
        /// 窗口资源配置类
        /// </summary>
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
        /// <summary>
        /// 窗口ID
        /// </summary>
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
        /// <summary>
        /// 打开窗口视图需要进行的处理
        /// </summary>
		public virtual void HandleAfterOpenView()
		{
		}

		public virtual void HandleBeforeCloseView()
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.signView_close, null, false);
		}
        /// <summary>
        /// 注册更新相关的处理回调
        /// </summary>
		public virtual void RegisterUpdateHandler()
		{
		}
        /// <summary>
        /// 注销更新相关的处理回调
        /// </summary>
		public virtual void CancelUpdateHandler()
		{
		}
        /// <summary>
        /// 刷新UI
        /// </summary>
		public virtual void RefreshUI()
		{
		}
        /// <summary>
        /// 重启界面
        /// </summary>
		public virtual void OnRestart()
		{
		}
        /// <summary>
        /// 销毁窗口接口
        /// </summary>
		public virtual void Destroy()
		{
			this.UnloadPrefabCache();
			this.uiWindow = null;
		}
        /// <summary>
        /// 数据更新接口
        /// </summary>
        /// <param name="data"></param>
		public virtual void DataUpdated(object data)
		{
			this.intiData = data;
		}
        /// <summary>
        /// 加载预制资源并缓存key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		protected GameObject LoadPrefabCache(string name)
		{
			GameObject result = ResourceManager.Load<GameObject>(name, true, this.WinResCfg.IsAssetbundle, null, 0, false);
			if (!this.prefabCacheKeys.Contains(name))
			{
				this.prefabCacheKeys.Add(name);
			}
			return result;
		}
        /// <summary>
        /// 卸载预制并清空缓存key列表
        /// </summary>
		protected void UnloadPrefabCache()
		{
			if (this.prefabCacheKeys != null)
			{
				string[] names = this.prefabCacheKeys.ToArray();
				ResourceManager.UnLoadBundle(names, this.WinResCfg.IsAssetbundle);
				this.prefabCacheKeys.Clear();
			}
		}
        /// <summary>
        /// 返回逻辑执行接口
        /// </summary>
        /// <returns></returns>
		public virtual bool DoReturnLogic()
		{
			return true;
		}
        /// <summary>
        /// 初始化接口
        /// </summary>
		public virtual void Initialize()
		{
			if (this.IsForceReset)//强制重置
			{
				this.BindObject();
				this.IsForceReset = false;
			}
			this.RegisterListener();
		}
        /// <summary>
        /// 处理隐藏事件接口
        /// </summary>
		public virtual void HandleHideEvent()
		{
			this.UnRegisterListener();
		}
        /// <summary>
        /// 处理销毁事件接口
        /// </summary>
		public virtual void HandleDestroyEvent()
		{
			this.HandleHideEvent();
			this.IsForceReset = true;
		}
        /// <summary>
        /// 重新绑定游戏对象//////
        /// </summary>
		protected virtual void BindObject()
		{
		}
        /// <summary>
        /// 注册事件侦听处理
        /// </summary>
		protected virtual void RegisterListener()
		{
		}
        /// <summary>
        /// 注销事件侦听处理
        /// </summary>
		protected virtual void UnRegisterListener()
		{
		}
	}
}
