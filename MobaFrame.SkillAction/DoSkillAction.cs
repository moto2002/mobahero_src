using System;

namespace MobaFrame.SkillAction
{
	public class DoSkillAction : BaseAction
	{
		public string skillId = string.Empty;

		protected override bool doAction()
		{
			if (base.unit != null && base.unit.skillManager != null)
			{
				base.unit.skillManager.TriggerBornPowerObj(this.skillId, base.unit.isPlayer);
			}
			this.Destroy();
			return true;
		}
	}
}
