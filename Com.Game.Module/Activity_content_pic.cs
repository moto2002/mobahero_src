using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_content_pic : Activity_contentBase
	{
		public UITexture uiTex;

		public UITweener tween;

		private Texture2D mTex;

		private Task loadTask;

		private void Awake()
		{
			this.mTex = new Texture2D(1197, 517, TextureFormat.RGB24, false);
			this.mTex.name = "LoginDownloadTexture";
		}

		private void OnDestroy()
		{
			if (this.mTex != null)
			{
				UnityEngine.Object.Destroy(this.mTex);
			}
			if (this.loadTask != null)
			{
				this.loadTask.Stop();
				this.loadTask = null;
			}
		}

		public override EActivityModuleType GetModuleType()
		{
			return EActivityModuleType.ePic;
		}

		[DebuggerHidden]
		public override IEnumerator RefreshUI(Func<IEnumerator> ieBreak)
		{
			Activity_content_pic.<RefreshUI>c__IteratorB7 <RefreshUI>c__IteratorB = new Activity_content_pic.<RefreshUI>c__IteratorB7();
			<RefreshUI>c__IteratorB.ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<$>ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<>f__this = this;
			return <RefreshUI>c__IteratorB;
		}

		private void OnLoadFinish(bool manual)
		{
			this.loadTask = null;
			if (!manual)
			{
			}
		}

		[DebuggerHidden]
		private IEnumerator Load()
		{
			Activity_content_pic.<Load>c__IteratorB8 <Load>c__IteratorB = new Activity_content_pic.<Load>c__IteratorB8();
			<Load>c__IteratorB.<>f__this = this;
			return <Load>c__IteratorB;
		}

		private void SetTexture()
		{
			if (this.uiTex != null)
			{
				this.uiTex.mainTexture = this.mTex;
				this.tween.enabled = true;
			}
		}
	}
}
