using GUIFramework;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class HeartView : BaseView<HeartView>
	{
		private AudioClip HeartSound;

		private AudioSourceControl m_audioSourceControl;

		private bool m_isOpen;

		public HeartView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "HeartView");
		}

		public override void Init()
		{
			base.Init();
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			if (!this.m_isOpen)
			{
				this.m_audioSourceControl = AudioMgr.Play(new AudioClipInfo
				{
					clipName = "sd_ui_heart",
					audioSourceType = eAudioSourceType.UI_Loop,
					fadeInSpeed = 0f,
					fadeOutSpeed = 1f,
					audioPriority = 10,
					volume = 1f
				}, AudioMgr.Instance.transform);
				this.m_isOpen = true;
			}
		}

		public override void HandleBeforeCloseView()
		{
			if (this.m_isOpen)
			{
				if (this.m_audioSourceControl != null)
				{
					this.m_audioSourceControl.FadeOut(1f);
				}
				this.m_isOpen = false;
			}
			base.HandleBeforeCloseView();
		}

		public override void Destroy()
		{
			base.Destroy();
			this.m_isOpen = false;
		}
	}
}
