using GUIFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace HUD.Module
{
	public class FPSModule : BaseModule
	{
		private CoroutineManager cMgr = new CoroutineManager();

		public FPSModule()
		{
			this.module = EHUDModule.FPS;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/FPSModule");
		}

		public override void Init()
		{
			base.Init();
			this.transform.GetComponent<TweenPosition>().ResetToBeginning();
		}

		public override void HandleAfterOpenModule()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23078, new MobaMessageFunc(this.OnMsg_QualityChange));
		}

		public override void HandleBeforeCloseModule()
		{
			this.cMgr.StopAllCoroutine();
			MobaMessageManager.UnRegistMessage((ClientMsg)23078, new MobaMessageFunc(this.OnMsg_QualityChange));
		}

		private void OnMsg_QualityChange(MobaMessage msg)
		{
			this.transform.GetComponent<UIPanel>().widgetsAreStatic = false;
			this.cMgr.StartCoroutine(this.Delay(), true);
		}

		[DebuggerHidden]
		private IEnumerator Delay()
		{
			FPSModule.<Delay>c__IteratorD8 <Delay>c__IteratorD = new FPSModule.<Delay>c__IteratorD8();
			<Delay>c__IteratorD.<>f__this = this;
			return <Delay>c__IteratorD;
		}
	}
}
