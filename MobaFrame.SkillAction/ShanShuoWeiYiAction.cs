using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class ShanShuoWeiYiAction : ShanShuoAction
	{
		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			return new ShanShuoWeiYiAction.<Coroutine>c__Iterator68();
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
