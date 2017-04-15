using System;

namespace MobaFrame.SkillAction
{
	public class AttachBuffAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (StringUtils.CheckValid(this.data.strParam1) && this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null && !SkillUtility.IsImmunityBuff(this.targetUnits[i], this.data.strParam1))
					{
						ActionManager.AddBuff(this.data.strParam1, this.targetUnits[i], base.unit, true, string.Empty);
					}
				}
			}
		}
	}
}
