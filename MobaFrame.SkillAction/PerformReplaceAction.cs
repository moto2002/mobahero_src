using System;

namespace MobaFrame.SkillAction
{
	public class PerformReplaceAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						SubPerformReplaceAction subPerformReplaceAction = new SubPerformReplaceAction
						{
							targetUnit = this.targetUnits[i],
							unit = base.unit,
							higheffId = this.higheffId
						};
						subPerformReplaceAction.Play();
						this.AddAction(subPerformReplaceAction);
					}
				}
			}
		}

		protected override void doStartHighEffect_Perform()
		{
		}
	}
}
