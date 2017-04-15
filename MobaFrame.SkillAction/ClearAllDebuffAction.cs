using System;

namespace MobaFrame.SkillAction
{
	public class ClearAllDebuffAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						this.targetUnits[i].buffManager.RemoveAllDebuff();
						this.targetUnits[i].highEffManager.ClearAllDebuffHighEff();
					}
					if (this.targetUnits[i].isPlayer)
					{
						this.targetUnits[i].skillManager.UpdateSkillUI();
					}
				}
			}
		}
	}
}
