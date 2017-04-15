using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class HuoYanBaoHongAction : SimpleSkillAction
	{
		protected override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			GlobalObject.Instance.StartCoroutine(this.DoSkillDamage(action, targets));
		}

		[DebuggerHidden]
		private IEnumerator DoSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			HuoYanBaoHongAction.<DoSkillDamage>c__Iterator64 <DoSkillDamage>c__Iterator = new HuoYanBaoHongAction.<DoSkillDamage>c__Iterator64();
			<DoSkillDamage>c__Iterator.action = action;
			<DoSkillDamage>c__Iterator.targets = targets;
			<DoSkillDamage>c__Iterator.<$>action = action;
			<DoSkillDamage>c__Iterator.<$>targets = targets;
			<DoSkillDamage>c__Iterator.<>f__this = this;
			return <DoSkillDamage>c__Iterator;
		}
	}
}
