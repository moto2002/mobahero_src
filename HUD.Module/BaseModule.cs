using GUIFramework;
using System;
using UnityEngine;

namespace HUD.Module
{
	public class BaseModule : IHUDModule
	{
		public WinResurceCfg WinResCfg
		{
			get;
			set;
		}

		public EHUDModule module
		{
			get;
			set;
		}

		public GameObject gameObject
		{
			get;
			set;
		}

		public Transform transform
		{
			get;
			set;
		}

		public virtual void Init()
		{
		}

		public virtual void Destroy()
		{
			UnityEngine.Object.Destroy(this.gameObject);
			this.transform = null;
			this.WinResCfg = null;
		}

		public virtual void HandleAfterOpenModule()
		{
		}

		public virtual void HandleBeforeCloseModule()
		{
		}

		public virtual void RegisterUpdateHandler()
		{
		}

		public virtual void CancelUpdateHandler()
		{
		}

		public virtual void SetActive(bool isActive)
		{
		}

		public virtual void onFlyOut()
		{
			if (this.transform != null)
			{
				TweenPosition component = this.transform.GetComponent<TweenPosition>();
				if (component != null)
				{
					component.PlayForward();
				}
			}
		}

		public virtual void onFlyIn()
		{
			if (this.transform != null)
			{
				TweenPosition component = this.transform.GetComponent<TweenPosition>();
				if (component != null)
				{
					component.PlayReverse();
				}
			}
		}

		public virtual void AdaptSkillPanelPivot()
		{
		}
	}
}
