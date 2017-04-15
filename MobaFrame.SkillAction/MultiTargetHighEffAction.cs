using System;

namespace MobaFrame.SkillAction
{
	public class MultiTargetHighEffAction<T> : BaseHighEffAction where T : BaseStateAction, new()
	{
		protected override void doStartHighEffect_Special()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null && !SkillUtility.IsImmunityHighEff(this.targetUnits[i], this.higheffId))
					{
						T t = BaseAction.CreateAction<T>();
						t.targetUnit = this.targetUnits[i];
						t.unit = base.unit;
						t.higheffId = this.higheffId;
						t.skillPosition = this.skillPosition;
						t.skillId = this.skillId;
						t.rotateY = this.rotateY;
						t.Play();
						this.AddAction(t);
					}
				}
			}
		}

		protected override void doStartHighEffect_Perform()
		{
		}
	}
}
