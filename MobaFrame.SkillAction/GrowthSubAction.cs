using Holoville.HOTween;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class GrowthSubAction : BaseStateAction
	{
		private Vector3 sourceScale;

		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.Growth.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.sourceScale = this.targetUnit.transform.localScale;
			this.targetUnit.Growth.Add();
			base.PlayEffects(this.targetUnit);
			float d = this.data.param1 + 1f;
			HOTween.To(this.targetUnit.transform, this.data.param2, new TweenParms().Prop("localScale", this.sourceScale * d).Ease(EaseType.EaseInOutBounce));
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.Growth.Remove();
			this.targetUnit.transform.localScale = this.sourceScale;
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			GrowthSubAction.<Coroutine>c__Iterator7F <Coroutine>c__Iterator7F = new GrowthSubAction.<Coroutine>c__Iterator7F();
			<Coroutine>c__Iterator7F.<>f__this = this;
			return <Coroutine>c__Iterator7F;
		}
	}
}
