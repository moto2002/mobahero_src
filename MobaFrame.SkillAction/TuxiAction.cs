using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class TuxiAction : BaseSkillAction
	{
		protected override bool doAction()
		{
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
			return true;
		}

		[DebuggerHidden]
		protected IEnumerator Coroutine()
		{
			TuxiAction.<Coroutine>c__Iterator6A <Coroutine>c__Iterator6A = new TuxiAction.<Coroutine>c__Iterator6A();
			<Coroutine>c__Iterator6A.<>f__this = this;
			return <Coroutine>c__Iterator6A;
		}

		protected override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			this.AddAction(ActionManager.HitSkill(action.skillKey, base.unit, targets, true));
			base.AddHighEff(action.skillKey, SkillPhrase.Hit, targets, this.targetPosition);
			base.AddBuff(action.skillKey, SkillPhrase.Hit, targets);
			base.OnSkillDamage(action, targets);
		}

		protected override void OnSkillEnd(BaseSkillAction action)
		{
			ActionManager.EndSkill(action.skillKey, base.unit, true);
			base.OnSkillEnd(action);
		}
	}
}
