using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros;
using System;

internal class Skill_Lanmao_04 : Skill
{
	public Skill_Lanmao_04(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override float GetCostValue(AttrType type)
	{
		if (base.data.cost_ids != null && base.data.cost_ids.Length > 0)
		{
			int id = base.data.cost_ids[0];
			DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(id);
			if (vo != null)
			{
				float mp_max = this.self.mp_max;
				return -(mp_max * vo.damageParam3 + vo.damageParam2);
			}
		}
		return 0f;
	}

	protected override void OnSkillInterrupt()
	{
		this.unit.ForceIdle();
		base.DestroyActions(SkillCastPhase.Cast_None);
		base.OnSkillInterrupt();
		Singleton<SkillView>.Instance.CheckIconToGrayByCanUse(null, -1);
	}
}
