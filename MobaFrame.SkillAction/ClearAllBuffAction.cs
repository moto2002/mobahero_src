using System;

namespace MobaFrame.SkillAction
{
	public class ClearAllBuffAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						string strParam = this.data.strParam1;
						this.targetUnits[i].buffManager.RemoveBuff(strParam, -1);
					}
				}
			}
		}
	}
}
