using System;

namespace MobaFrame.SkillAction
{
	public class BecomeDeadAction : BaseAction
	{
		public Units targetUnit;

		protected override void OnSendStart()
		{
		}

		protected override void OnRecordStart()
		{
			int arg_22_0 = (!(base.unit != null)) ? 0 : base.unit.unique_id;
		}

		protected override bool doAction()
		{
			this.targetUnit.isLive = false;
			this.targetUnit.PreDeath(base.unit);
			this.targetUnit.RealDeath(base.unit);
			return true;
		}
	}
}
