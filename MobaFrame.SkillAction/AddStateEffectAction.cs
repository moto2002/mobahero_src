using System;

namespace MobaFrame.SkillAction
{
	public class AddStateEffectAction : BaseHighEffAction
	{
		protected override void StartHighEff()
		{
			this.Destroy();
		}

		protected override void StopHighEff()
		{
		}
	}
}
