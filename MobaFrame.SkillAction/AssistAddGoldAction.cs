using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class AssistAddGoldAction : BaseHighEffAction
	{
		protected override void StartHighEff()
		{
			if (UtilManager.Instance == null)
			{
				return;
			}
			if (this.data.param1 > 0f)
			{
				MobaMessageManager.RegistMessage((ClientMsg)25056, new MobaMessageFunc(this.DoUnitDead));
			}
		}

		private void DoUnitDead(MobaMessage msg)
		{
			ParamUnitDead paramUnitDead = msg.Param as ParamUnitDead;
			if (paramUnitDead != null)
			{
				if (paramUnitDead.attacker == null || paramUnitDead.target == null || base.unit == null)
				{
					return;
				}
				if ((base.unit.isHero || base.unit.isPlayer) && (paramUnitDead.attacker.isHero || paramUnitDead.attacker.isPlayer))
				{
					if (base.unit == paramUnitDead.attacker || paramUnitDead.target.isHero || paramUnitDead.target.isPlayer || paramUnitDead.target.isHome || paramUnitDead.target.isTower)
					{
						return;
					}
					if (TeamManager.CheckTeam(base.unit.gameObject, paramUnitDead.attacker.gameObject, SkillTargetCamp.Partener, null) && Vector3.Distance(base.unit.transform.position, paramUnitDead.attacker.transform.position) < this.data.param1)
					{
						for (int i = 0; i < this.data.attachHighEffs.Length; i++)
						{
							string text = this.data.attachHighEffs[i];
							if (text != null)
							{
								if (StringUtils.CheckValid(text))
								{
									ActionManager.AddHighEffect(text, this.skillId, base.unit, base.unit, null, true);
								}
							}
						}
					}
				}
			}
		}

		protected override void OnDestroy()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)25056, new MobaMessageFunc(this.DoUnitDead));
			base.OnDestroy();
		}
	}
}
