using System;

namespace MobaFrame.SkillAction
{
	public class BaoZhaAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						base.PlayEffects(this.targetUnits[i]);
					}
				}
			}
			if (this.data.strParam1 != null)
			{
				this.AddAction(ActionManager.PlayEffect(this.data.strParam1, base.unit, null, null, true, string.Empty, base.unit));
			}
		}

		protected override void doStartHighEffect_End()
		{
			if (base.IsAutoDestroy)
			{
				this.Destroy();
			}
		}
	}
}
