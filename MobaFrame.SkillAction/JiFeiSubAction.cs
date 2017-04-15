using Com.Game.Module;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class JiFeiSubAction : BaseStateAction
	{
		private AnimationCurve curve = new AnimationCurve();

		private Vector3 sourcePos = new Vector3(99999f, 0f, 11111f);

		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.JiFei.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.JiFei.Add();
			base.EnableAction(this.targetUnit, false, this.data.param1);
			float value = 3f;
			if (this.data.param2 != 0f)
			{
				value = this.data.param2;
			}
			if (this.data.param1 >= 1f)
			{
				this.curve.AddKey(0f, 0f);
				this.curve.AddKey(0.3f, value);
				this.curve.AddKey(0.6f, value);
				this.curve.AddKey(1f, 0f);
			}
			else
			{
				this.curve.AddKey(0f, 0f);
				this.curve.AddKey(0.3f * this.data.param1, value);
				this.curve.AddKey(0.6f * this.data.param1, value);
				this.curve.AddKey(1f * this.data.param1, 0f);
			}
			this.sourcePos = this.targetUnit.mTransform.position;
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			this.targetUnit.ShowDebuffIcon(true, 116);
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitBeJifei, this.targetUnit, null, null);
		}

		protected override void RevertState()
		{
			base.RevertState();
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			JiFeiSubAction.<Coroutine>c__Iterator80 <Coroutine>c__Iterator = new JiFeiSubAction.<Coroutine>c__Iterator80();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}

		protected override void OnDestroy()
		{
			if (this.targetUnit != null)
			{
				this.targetUnit.JiFei.Remove();
				if (!this.targetUnit.JiFei.IsInState)
				{
					this.sourcePos = this.targetUnit.mTransform.position;
					this.targetUnit.PlayAnim(AnimationType.HitStun, false, 0, true, false);
					this.targetUnit.SetPosition(this.sourcePos, false);
					base.EnableAction(this.targetUnit, true, this.data.param1);
					Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitUnderGroud, this.targetUnit, null, null);
				}
				this.targetUnit.ShowDebuffIcon(false, 116);
			}
			base.OnDestroy();
		}
	}
}
