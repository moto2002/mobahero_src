using GUIFramework;
using System;

namespace Com.Game.Module
{
	public class TemplateView : BaseView<TemplateView>
	{
		public TemplateView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "TemplateView");
		}

		public override void Init()
		{
			base.Init();
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}
	}
}
