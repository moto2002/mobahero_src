using Com.Game.Utils;
using GUIFramework;
using MobaProtocol;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Com.Game.Module
{
	public class TipView : BaseView<TipView>
	{
		private UILabel Title;

		private UILabel Text;

		private Transform ItemView;

		private TweenAlpha TweenAnim;

		private Thread AutoCloseThread;

		private string text_str;

		private TimeoutController time_out;

		public TipView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/TipView");
		}

		public override void Init()
		{
			base.Init();
			this.Text = this.transform.Find("Anchor/Text").GetComponent<UILabel>();
			this.TweenAnim = this.transform.GetComponent<TweenAlpha>();
			this.time_out = this.transform.GetComponent<TimeoutController>();
			if (this.time_out == null)
			{
				this.time_out = this.gameObject.AddComponent<TimeoutController>();
			}
			this.TweenAnim.ResetToBeginning();
		}

		public override void HandleAfterOpenView()
		{
			if (this.TweenAnim != null)
			{
				this.TweenAnim.ResetToBeginning();
				this.TweenAnim.Begin();
			}
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
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

		public void SetText(string text, float timeValue = 0f)
		{
			this.text_str = text;
			if (timeValue != 0f)
			{
				this.SetTime(timeValue);
			}
		}

		public void SetTime(float value)
		{
			this.time_out.StartTimeOut(value, new Callback(this.CloseViewWait));
		}

		public void CloseViewWait()
		{
			CtrlManager.CloseWindow(WindowID.TipView);
		}

		public void ShowViewSetText(string text, float time = 1f)
		{
			this.text_str = text;
			CtrlManager.OpenWindow(WindowID.TipView, null);
			if (this.time_out == null)
			{
				this.time_out = this.transform.GetComponent<TimeoutController>();
			}
			if (this.time_out == null)
			{
				this.time_out = this.gameObject.AddComponent<TimeoutController>();
			}
			this.SetTime(time);
		}

		public void GetErrorInformation(int arg1)
		{
			IEnumerator enumerator = Enum.GetValues(typeof(MobaErrorCode)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MobaErrorCode mobaErrorCode = (MobaErrorCode)((int)enumerator.Current);
					if (arg1 == (int)mobaErrorCode)
					{
						if (arg1 != 0)
						{
							ClientLogger.Error("错误信息:" + mobaErrorCode.ToString());
						}
						break;
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
