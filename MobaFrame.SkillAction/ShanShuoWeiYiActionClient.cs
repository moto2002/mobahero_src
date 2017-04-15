using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class ShanShuoWeiYiActionClient : ShanShuoAction
	{
		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			ShanShuoWeiYiActionClient.<Coroutine>c__Iterator69 <Coroutine>c__Iterator = new ShanShuoWeiYiActionClient.<Coroutine>c__Iterator69();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}

		protected override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			this.AddAction(ActionManager.HitSkill(action.skillKey, base.unit, targets, true));
			base.AddHighEff(action.skillKey, SkillPhrase.Hit, targets, this.targetPosition);
			base.AddBuff(action.skillKey, SkillPhrase.Hit, targets);
			base.OnSkillDamage(action, targets);
		}
	}
}
