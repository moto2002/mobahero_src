using System;

namespace MobaFrame.SkillAction
{
	public class FengBaoZhiYanAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Perform()
		{
			if (this.performIds != null)
			{
				for (int i = 0; i < this.performIds.Length; i++)
				{
					if (StringUtils.CheckValid(this.performIds[i]))
					{
						if (i == 0)
						{
							base.PlayPerform(this.performIds[i], base.unit, this.targetUnits);
						}
						else if (i == 1)
						{
							base.PlayPerform(this.performIds[i], base.unit, this.targetUnits);
						}
						else if (i == 2 && this.targetUnits != null)
						{
							for (int j = 0; j < this.targetUnits.Count; j++)
							{
								if (this.targetUnits[j] != null)
								{
									base.PlayEffects(this.performIds[i], this.targetUnits[j]);
								}
							}
						}
					}
				}
			}
		}
	}
}
