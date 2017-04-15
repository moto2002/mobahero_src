using System;

namespace MobaFrame.SkillAction
{
	public class AttackForTargeBuffAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						this.targetUnits[i].attackForTargetBuff = this.data.strParam1;
					}
				}
			}
		}
	}
}
