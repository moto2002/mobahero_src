using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;

public class Skill_Xiaohei_01 : Skill
{
	public List<Units> hitList = new List<Units>();

	public Skill_Xiaohei_01(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void StartCastSkill(SkillDataKey skill_key)
	{
		this.hitList.Clear();
		base.StartCastSkill(skill_key);
	}

	public override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
	{
		if (!Singleton<PvpManager>.Instance.IsInPvp)
		{
			List<Units> list = new List<Units>(targets);
			if (targets != null)
			{
				for (int i = 0; i < targets.Count; i++)
				{
					if (!this.hitList.Contains(targets[i]))
					{
						this.hitList.Add(targets[i]);
					}
					else
					{
						list.Remove(targets[i]);
					}
				}
			}
			base.OnSkillDamage(action, list);
		}
		else
		{
			base.OnSkillDamage(action, targets);
		}
	}
}
