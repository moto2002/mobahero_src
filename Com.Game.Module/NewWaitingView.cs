using GUIFramework;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class NewWaitingView : BaseView<NewWaitingView>
	{
		private const float waitTime_toShow = 0.5f;

		private UILabel label_text;

		private UIPanel panel;

		private GameObject goInBattle;

		private GameObject goNormal;

		private object[] msgs;

		private bool bNormal = true;

		private CoroutineCom cCom;

		public NewWaitingView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/WaitingView");
		}

		public override void Init()
		{
			base.Init();
			this.msgs = new object[]
			{
				ClientC2V.WaitingView_text,
				ClientC2V.WaitingView_show,
				ClientC2V.WaitingView_normal
			};
			this.transform.MyGetCompoent(null, out this.panel);
			this.transform.MyGetCompoent("Anchor/inBattle/Text", out this.label_text);
			this.goInBattle = this.transform.FindChild("Anchor/inBattle").gameObject;
			this.goNormal = this.transform.FindChild("Anchor/normal").gameObject;
			this.cCom = this.transform.FindChild("Anchor").GetComponent<CoroutineCom>();
			this.Regist();
		}

		public override void HandleAfterOpenView()
		{
			this.ShowSelf(false);
			this.cCom.End("delay");
			this.cCom.Begin("delay", this.DelayShow());
			this.label_text.text = string.Empty;
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void Destroy()
		{
			this.Unregist();
			base.Destroy();
		}

		private void Regist()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		private void Unregist()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		private void ShowSelf(bool bShow)
		{
			this.panel.alpha = ((!bShow) ? 0.01f : 1f);
		}

		private void RefreshUI_normal()
		{
			this.goInBattle.SetActive(!this.bNormal);
			this.goNormal.SetActive(this.bNormal);
		}

		[DebuggerHidden]
		private IEnumerator DelayShow()
		{
			NewWaitingView.<DelayShow>c__Iterator125 <DelayShow>c__Iterator = new NewWaitingView.<DelayShow>c__Iterator125();
			<DelayShow>c__Iterator.<>f__this = this;
			return <DelayShow>c__Iterator;
		}

		private void OnMsg_WaitingView_text(MobaMessage msg)
		{
			string text = msg.Param as string;
			if (string.IsNullOrEmpty(text))
			{
				this.label_text.text = string.Empty;
			}
			else
			{
				this.label_text.text = text;
			}
		}

		private void OnMsg_WaitingView_show(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				bool flag = !(bool)msg.Param;
				if (flag)
				{
					this.ShowSelf(true);
				}
			}
		}

		private void OnMsg_WaitingView_normal(MobaMessage msg)
		{
			bool flag = (bool)msg.Param;
			this.bNormal = flag;
			this.RefreshUI_normal();
		}
	}
}
