using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class TweenRotateAction : BaseAction
	{
		public float fromAngleDegree;

		public float toAngleDegree;

		public float tweenTime;

		private float _birthTime;

		private float _pastTime;

		private float _rate;

		protected override bool doAction()
		{
			if (this.tweenTime < 0.001f)
			{
				return false;
			}
			this._birthTime = Time.time;
			base.mCoroutineManager.StartCoroutine(this.TweenRotateCoroutine(), true);
			return true;
		}

		[DebuggerHidden]
		private IEnumerator TweenRotateCoroutine()
		{
			TweenRotateAction.<TweenRotateCoroutine>c__Iterator90 <TweenRotateCoroutine>c__Iterator = new TweenRotateAction.<TweenRotateCoroutine>c__Iterator90();
			<TweenRotateCoroutine>c__Iterator.<>f__this = this;
			return <TweenRotateCoroutine>c__Iterator;
		}
	}
}
