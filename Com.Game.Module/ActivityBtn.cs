using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class ActivityBtn : MonoBehaviour
	{
		public enum EActivityBtn
		{
			eNone,
			eGoto,
			eReward,
			eFinish,
			eInactive
		}

		public ActivityBtn.EActivityBtn eBtnType;

		public UISprite sp_normal;

		public UISprite sp_press;

		public UILabel lb_text;

		public TweenAlpha[] tws;

		public Action<ActivityBtn> callback;

		private bool bPress;

		private void Awake()
		{
			this.RefreshUI_btn();
		}

		private void Start()
		{
			UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.MyOnClick);
			UIEventListener.Get(base.gameObject).onPress = new UIEventListener.BoolDelegate(this.MyOnPress);
		}

		private void MyOnClick(GameObject go)
		{
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		private void MyOnPress(GameObject go, bool state)
		{
			if (state)
			{
				this.bPress = true;
			}
			else
			{
				this.bPress = false;
			}
			this.RefreshUI_btn();
		}

		private void RefreshUI_btn()
		{
			if (this.sp_normal != this.sp_press && this.sp_normal != null && this.sp_press != null)
			{
				this.sp_normal.gameObject.SetActive(!this.bPress);
				this.sp_press.gameObject.SetActive(this.bPress);
			}
		}

		public void RefreshUI_alpha(float alpha = 0.01f)
		{
			this.sp_normal.alpha = alpha;
			this.lb_text.alpha = alpha;
		}

		public void RefreshUI_tweenAlpha(bool b)
		{
			for (int i = 0; i < this.tws.Length; i++)
			{
				this.tws[i].enabled = b;
			}
		}
	}
}
