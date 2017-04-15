using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_rewardItem : MonoBehaviour
	{
		public UISprite sp_frame;

		public UISprite sp_icon;

		public UITexture te_icon;

		public UISprite sp_discount3;

		public UISprite sp_discount7;

		public UILabel lb_num;

		private bool bDrag;

		public Action<Activity_rewardItem, bool> OnMouseOver
		{
			get;
			set;
		}

		public RewardItemBase Info
		{
			get;
			set;
		}

		private void Awake()
		{
			this.bDrag = false;
		}

		private void Start()
		{
			UIEventListener.Get(base.gameObject).onMobileHover = new UIEventListener.BoolDelegate(this.MyOnMobileHover);
		}

		[DebuggerHidden]
		public IEnumerator RefreshUI(IEnumerator rewardRepos)
		{
			Activity_rewardItem.<RefreshUI>c__IteratorC2 <RefreshUI>c__IteratorC = new Activity_rewardItem.<RefreshUI>c__IteratorC2();
			<RefreshUI>c__IteratorC.rewardRepos = rewardRepos;
			<RefreshUI>c__IteratorC.<$>rewardRepos = rewardRepos;
			<RefreshUI>c__IteratorC.<>f__this = this;
			return <RefreshUI>c__IteratorC;
		}

		private void RefreshUI_alpha(float alpha = 0.01f)
		{
			UIWidget[] componentsInChildren = base.transform.GetComponentsInChildren<UIWidget>(true);
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].alpha = alpha;
				}
			}
		}

		private void RefreshUI_tweenAlpha(bool b)
		{
			TweenAlpha[] componentsInChildren = base.transform.GetComponentsInChildren<TweenAlpha>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = b;
				}
			}
		}

		private void MyOnMobileHover(GameObject go, bool state)
		{
			if (!this.bDrag && this.OnMouseOver != null)
			{
				this.OnMouseOver(this, state);
			}
		}
	}
}
