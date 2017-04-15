using System;

namespace MobaFrame.SkillAction
{
	public class SubBuffLayerEffect : BaseHighEffAction
	{
		public Units targetUnit;

		protected override void StartHighEff()
		{
			if (this.targetUnit == null)
			{
				return;
			}
			this.Destroy();
		}

		protected override void StopHighEff()
		{
		}
	}
}
