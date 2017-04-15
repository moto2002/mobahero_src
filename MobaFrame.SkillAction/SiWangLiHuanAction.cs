using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class SiWangLiHuanAction : SimpleSkillAction
	{
		protected override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			if (targets == null)
			{
				return;
			}
			List<Units> list = new List<Units>();
			for (int i = 0; i < targets.Count; i++)
			{
				if (i < 3)
				{
					list.Add(targets[i]);
				}
			}
			base.OnSkillDamage(action, list);
		}
	}
}
