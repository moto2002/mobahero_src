using GUIFramework;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Com.Game.Module
{
	public class UnlockView : BaseView<UnlockView>
	{
		private UILabel Title;

		private UILabel Text;

		private Transform ItemView;

		private Transform ClickArea;

		private Thread AutoCloseThread;

		private string text_str = string.Empty;

		private TimeoutController time_out;

		private bool canClick;

		private string effectId = string.Empty;

		private int showType = 1;

		private int showIndex;

		private List<string[]> infoList = new List<string[]>();

		public UnlockView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/UnlockView");
		}

		public override void Init()
		{
			base.Init();
			this.Text = this.transform.Find("Anchor/Text").GetComponent<UILabel>();
			this.ClickArea = this.transform.Find("Anchor/ClickArea");
			this.time_out = this.transform.GetComponent<TimeoutController>();
			if (this.time_out == null)
			{
				this.time_out = this.gameObject.AddComponent<TimeoutController>();
			}
		}

		public override void HandleAfterOpenView()
		{
			if (this.time_out != null)
			{
				this.time_out.StartTimeOut(1f, new Callback(this.EnableClick));
			}
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			if (this.ClickArea.gameObject.GetComponent<UIEventListener>() == null)
			{
				this.ClickArea.gameObject.AddComponent<UIEventListener>();
			}
			UIEventListener expr_3C = UIEventListener.Get(this.ClickArea.gameObject);
			expr_3C.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_3C.onClick, new UIEventListener.VoidDelegate(this.CloseViewClick));
			UIEventListener expr_68 = UIEventListener.Get(this.gameObject);
			expr_68.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_68.onClick, new UIEventListener.VoidDelegate(this.CloseViewClick));
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
			UIEventListener expr_10 = UIEventListener.Get(this.ClickArea.gameObject);
			expr_10.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(expr_10.onClick, new UIEventListener.VoidDelegate(this.CloseViewClick));
			UIEventListener expr_3C = UIEventListener.Get(this.gameObject);
			expr_3C.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(expr_3C.onClick, new UIEventListener.VoidDelegate(this.CloseViewClick));
		}

		public override void RefreshUI()
		{
			this.Text.text = this.text_str;
		}

		public override void Destroy()
		{
			this.text_str = string.Empty;
			this.time_out.StopTimeOut();
			base.Destroy();
		}

		public void SetText(string text, string effectId = "")
		{
			if (this.text_str == string.Empty)
			{
				this.infoList.Clear();
				this.showIndex = 0;
				this.text_str = text;
				this.SetType(1);
				this.infoList.Add(new string[]
				{
					text,
					string.Empty
				});
			}
			else
			{
				this.SetType(2);
				this.infoList.Add(new string[]
				{
					text,
					string.Empty
				});
			}
		}

		public void SetText(List<string[]> _infoList)
		{
			this.SetType(2);
			this.showIndex = 0;
			this.infoList = _infoList;
			this.text_str = this.infoList[this.showIndex][0];
			this.effectId = this.infoList[this.showIndex][1];
		}

		private void SetType(int _showType)
		{
			this.showType = _showType;
		}

		public void CloseViewClick(GameObject obj)
		{
			if (this.canClick)
			{
				this.canClick = false;
				CtrlManager.CloseWindow(WindowID.UnlockView);
				this.text_str = string.Empty;
				this.CheckShowType();
			}
		}

		private void CheckShowType()
		{
			if (this.showType == 2 && this.showIndex < this.infoList.Count - 1)
			{
				this.showIndex++;
				this.text_str = this.infoList[this.showIndex][0];
				this.effectId = this.infoList[this.showIndex][1];
				this.canClick = true;
				CtrlManager.OpenWindow(WindowID.UnlockView, null);
			}
		}

		public void EnableClick()
		{
			this.canClick = true;
		}
	}
}
