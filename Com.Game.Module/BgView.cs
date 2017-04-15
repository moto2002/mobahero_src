using GUIFramework;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class BgView : BaseView<BgView>
	{
		private UITexture bgTexture;

		private string pathName;

		public BgView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/BgView");
		}

		public override void Init()
		{
			base.Init();
			this.bgTexture = this.transform.Find("BG").GetComponent<UITexture>();
			this.SetBg();
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23013, new MobaMessageFunc(this.On_setBg));
		}

		public override void HandleAfterOpenView()
		{
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23013, new MobaMessageFunc(this.On_setBg));
		}

		public override void HandleBeforeCloseView()
		{
			if (this.bgTexture != null)
			{
				this.bgTexture.mainTexture = null;
			}
		}

		private void On_setBg(MobaMessage msg)
		{
			this.pathName = (msg.Param as string);
			this.SetBg();
		}

		private void SetBg()
		{
			Texture2D texture2D = null;
			if (!string.IsNullOrEmpty(this.pathName))
			{
				texture2D = (Resources.Load(this.pathName) as Texture2D);
			}
			if (texture2D != null)
			{
				this.bgTexture.mainTexture = texture2D;
			}
		}

		public void SetBgPath(string _pathName)
		{
			this.pathName = _pathName;
		}
	}
}
