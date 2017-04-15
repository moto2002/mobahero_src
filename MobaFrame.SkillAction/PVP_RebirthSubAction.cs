using System;

namespace MobaFrame.SkillAction
{
	public class PVP_RebirthSubAction : RebirthSubAction
	{
		protected override void StartHighEff()
		{
			this.targetUnit.PreDeath(null);
			this.doReBirth();
		}

		protected override void doReBirth()
		{
			base.mCoroutineManager.StartCoroutine(base.Rebirth_Coroutinue(), true);
		}
	}
}
