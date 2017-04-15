using MobaHeros;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class DelayDisplayEyeItemAction : BaseAction
	{
		public string preEffectResource = string.Empty;

		public Vector3 originalPos = Vector3.zero;

		public Vector3 targetPos = Vector3.zero;

		public float lifeTime;

		public GameObject eyeItemObj;

		private ResourceHandle _resHandle;

		private float _birthTime;

		private float _pastTime;

		private float _rate;

		protected override void OnInit()
		{
			if (this.eyeItemObj != null)
			{
				this.eyeItemObj.SetActive(false);
			}
			this.SpawnPreDisplayEffect(this.preEffectResource);
		}

		protected override void OnStop()
		{
			if (this._resHandle != null)
			{
				this._resHandle.Release();
				this._resHandle = null;
			}
			if (this.eyeItemObj != null)
			{
				this.eyeItemObj.SetActive(true);
				this.eyeItemObj = null;
			}
		}

		protected override bool doAction()
		{
			if (this.lifeTime < 0.001f)
			{
				return false;
			}
			this._birthTime = Time.time;
			base.mCoroutineManager.StartCoroutine(this.PreEffectFlyCoroutinue(), true);
			return true;
		}

		[DebuggerHidden]
		private IEnumerator PreEffectFlyCoroutinue()
		{
			DelayDisplayEyeItemAction.<PreEffectFlyCoroutinue>c__Iterator8F <PreEffectFlyCoroutinue>c__Iterator8F = new DelayDisplayEyeItemAction.<PreEffectFlyCoroutinue>c__Iterator8F();
			<PreEffectFlyCoroutinue>c__Iterator8F.<>f__this = this;
			return <PreEffectFlyCoroutinue>c__Iterator8F;
		}

		private void SpawnPreDisplayEffect(string effectId)
		{
			if (!StringUtils.CheckValid(effectId))
			{
				return;
			}
			this._resHandle = MapManager.Instance.SpawnResourceHandle(effectId, null, 0);
			if (this._resHandle == null)
			{
				return;
			}
			this._resHandle.Raw.position = this.originalPos;
			this._resHandle.Raw.LookAt(this.targetPos);
		}
	}
}
