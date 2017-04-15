using GUIFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class ProgressView : BaseView<ProgressView>
	{
		private const float waitTime_toShow = 0.5f;

		private UILabel label_text;

		private UIPanel panel;

		private bool bNormal = true;

		private CoroutineCom cCom;

		public ProgressView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/ProgressView");
		}

		public override void Init()
		{
			base.Init();
			this.transform.MyGetCompoent(null, out this.panel);
			this.transform.MyGetCompoent("Anchor/inBattle/Text", out this.label_text);
			this.cCom = this.transform.FindChild("Anchor").GetComponent<CoroutineCom>();
		}

		public override void HandleAfterOpenView()
		{
			this.ShowSelf(false);
			this.cCom.End("delay");
			this.cCom.Begin("delay", this.DelayShow());
			this.label_text.text = string.Empty;
		}

		private void ShowSelf(bool bShow)
		{
			this.panel.alpha = ((!bShow) ? 0.01f : 1f);
		}

		[DebuggerHidden]
		private IEnumerator DelayShow()
		{
			ProgressView.<DelayShow>c__Iterator127 <DelayShow>c__Iterator = new ProgressView.<DelayShow>c__Iterator127();
			<DelayShow>c__Iterator.<>f__this = this;
			return <DelayShow>c__Iterator;
		}

		public static void ShowProgressText(string text)
		{
			if (!Singleton<ProgressView>.Instance.IsOpen)
			{
				CtrlManager.OpenWindow(WindowID.ProgressView, null);
			}
			Singleton<ProgressView>.Instance.label_text.text = text;
		}
	}
}
