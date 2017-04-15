using GUIFramework;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class StartView_New : BaseView<StartView_New>
	{
		private Transform bg;

		private Transform warn;

		private Coroutine coroutine;

		public StartView_New()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Login/StartView");
		}

		public override void Init()
		{
			base.Init();
			this.bg = this.transform.Find("Anchor/Warn/Bg");
			this.warn = this.transform.Find("Anchor/Warn");
			this.warn.gameObject.SetActive(false);
			this.bg.gameObject.SetActive(true);
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.warn.gameObject.SetActive(false);
			this.bg.gameObject.SetActive(false);
		}

		public override void HandleBeforeCloseView()
		{
		}

		public void ShowWarn()
		{
			this.warn.gameObject.SetActive(true);
			this.bg.gameObject.SetActive(false);
		}

		public override void Destroy()
		{
			this.bg = null;
			this.warn = null;
			base.Destroy();
		}
	}
}
