using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class AttackExtraDamageAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			this.data.param1 = Mathf.Clamp(this.data.param1, 0f, 3.40282347E+38f);
			this.data.param2 = Mathf.Clamp(this.data.param2, 1f, 100f);
			this.data.param3 = Mathf.Clamp(this.data.param3, 0f, 100f);
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						this.targetUnits[i].attackExtraDamage = this.data.param1;
						this.targetUnits[i].attackMultipleDamage = this.data.param2;
						this.targetUnits[i].beheadedCoefficient = this.data.param3;
						this.targetUnits[i].attackReboundCoefficient = this.data.param4;
					}
				}
			}
		}
	}
}
