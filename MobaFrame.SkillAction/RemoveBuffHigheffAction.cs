using System;

namespace MobaFrame.SkillAction
{
	public class RemoveBuffHigheffAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						ActionManager.RemoveBuff(this.data.strParam1, this.targetUnits[i], this.targetUnits[i], -1, true);
					}
				}
			}
		}
	}
}
