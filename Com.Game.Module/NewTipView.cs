using GUIFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class NewTipView : BaseView<NewTipView>
	{
		private UILabel Text;

		private TweenAlpha TweenAnim;

		private CoroutineManager T_coroutineManager = new CoroutineManager();

		private string T_text = string.Empty;

		private float T_delayTime = 2f;

		private Callback<object> T_callBack;

		public NewTipView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/TipView");
		}

		public override void Init()
		{
			base.Init();
			this.Text = this.transform.Find("Anchor/Text").GetComponent<UILabel>();
			this.TweenAnim = this.transform.GetComponent<TweenAlpha>();
			this.TweenAnim.ResetToBeginning();
		}

		public override void HandleAfterOpenView()
		{
			if (this.TweenAnim != null)
			{
				this.TweenAnim.Begin();
			}
			this.StopAllCorountineManager();
		}

		public override void HandleBeforeCloseView()
		{
			if (this.TweenAnim != null)
			{
				this.TweenAnim.ResetToBeginning();
			}
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void ShowTipView(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			TipViewMessage tipViewMessage = (TipViewMessage)msg.Param;
			if (tipViewMessage == null)
			{
				return;
			}
			this.SetData(tipViewMessage.Text, tipViewMessage.DelayTime, tipViewMessage.CallBack);
			this.SetView();
			this.StartNewCorountine();
		}

		private void SetData(string str = "", float time = 2f, Callback<object> callBack = null)
		{
			this.T_text = ((str != null && !(str == string.Empty)) ? str : string.Empty);
			this.T_delayTime = ((time != 0f) ? time : 2f);
			this.T_callBack = callBack;
		}

		private void SetView()
		{
			this.Text.text = this.T_text;
		}

		private void StartNewCorountine()
		{
			this.T_coroutineManager.StartCoroutine(this.TipAnimation(this.T_delayTime), true);
		}

		[DebuggerHidden]
		private IEnumerator TipAnimation(float time)
		{
			NewTipView.<TipAnimation>c__Iterator124 <TipAnimation>c__Iterator = new NewTipView.<TipAnimation>c__Iterator124();
			<TipAnimation>c__Iterator.time = time;
			<TipAnimation>c__Iterator.<$>time = time;
			<TipAnimation>c__Iterator.<>f__this = this;
			return <TipAnimation>c__Iterator;
		}

		private void StopAllCorountineManager()
		{
			this.T_coroutineManager.StopAllCoroutine();
		}
	}
}
