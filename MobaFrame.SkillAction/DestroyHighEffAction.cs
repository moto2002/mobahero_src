using System;

namespace MobaFrame.SkillAction
{
	public class DestroyHighEffAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						this.targetUnits[i].highEffManager.RemoveHighEffect(this.data.strParam1);
					}
				}
			}
		}

		protected override void doStartHighEffect_Perform()
		{
		}
	}
}
