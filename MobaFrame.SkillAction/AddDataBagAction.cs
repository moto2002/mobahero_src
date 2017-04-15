using System;

namespace MobaFrame.SkillAction
{
	public class AddDataBagAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			int damage_id = (int)this.data.param1;
			for (int i = 0; i < this.targetUnits.Count; i++)
			{
				if (this.data.param2 != 0f)
				{
					if (this.data.param2 == 1f)
					{
						if (this.data.strParam1 == string.Empty)
						{
							this.Destroy();
							return;
						}
						if (this.targetUnits[i].GetAttackTarget() == null)
						{
							this.Destroy();
							return;
						}
						if (!this.targetUnits[i].GetAttackTarget().buffManager.IsHaveBuff(this.data.strParam1))
						{
							this.Destroy();
							return;
						}
					}
				}
				this.targetUnits[i].dataChange.InsertDataBag(damage_id);
			}
		}
	}
}
