using GUIFramework;
using System;

namespace Com.Game.Module
{
	public class MainBgCtrl : BaseView<MainBgCtrl>
	{
		private UISprite bgTexture;

		public MainBgCtrl()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/MainBgView");
		}

		public override void Init()
		{
			this.bgTexture = this.transform.Find("BG").GetComponent<UISprite>();
			base.Init();
		}

		public override void Destroy()
		{
			if (this.bgTexture != null && this.bgTexture.atlas != null && this.bgTexture.atlas.spriteMaterial != null && this.bgTexture.atlas.spriteMaterial.mainTexture != null)
			{
				HomeGCManager.Instance.UnloadAsset(this.bgTexture.atlas.spriteMaterial.mainTexture);
			}
		}
	}
}
