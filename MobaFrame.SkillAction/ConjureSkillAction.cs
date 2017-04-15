using Com.Game.Module;
using MobaHeros.Pvp;
using System;

namespace MobaFrame.SkillAction
{
	public class ConjureSkillAction : BaseHighEffAction
	{
		protected override void doStartHighEffect_Special()
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return;
			}
			if (base.unit != null)
			{
				string strParam = this.data.strParam1;
				if (this.targetUnits != null && this.targetUnits.Count > 0)
				{
					base.unit.Conjure(strParam, this.targetUnits[0], null);
				}
			}
		}
	}
}
