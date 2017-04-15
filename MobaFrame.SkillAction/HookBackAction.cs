using System;

namespace MobaFrame.SkillAction
{
	public class HookBackAction : BaseHighEffAction
	{
		protected override void StartHighEff()
		{
			base.StartHighEff();
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						base.EnableAction(this.targetUnits[i], false, 0f);
						this.targetUnits[i].InterruptAction(SkillInterruptType.Passive);
					}
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						base.EnableAction(this.targetUnits[i], true, 0f);
					}
				}
			}
		}
	}
}
