using System;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Yasuo_03 : Skill
{
	private const string unitTargetedBuff = "Skill_Aier_03";

	public Skill_Yasuo_03()
	{
	}

	public Skill_Yasuo_03(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void DoSkillLevelUp()
	{
		base.DoSkillLevelUp();
	}

	public override Units ReselectTarget(Units target, bool isCrazyMode = true)
	{
		if (target != null && target.isTower)
		{
			target = null;
		}
		if (target != null && target.buffManager != null)
		{
			if (!target.buffManager.IsHaveBuff("Skill_Aier_03"))
			{
				return target;
			}
			if (!isCrazyMode)
			{
				return null;
			}
		}
		float num = 9999999f;
		Units units = null;
		float num2 = 9999999f;
		Units units2 = null;
		Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
		foreach (Units current in allMapUnits.Values)
		{
			if (current.teamType != this.unit.teamType && current.CanBeSelectedManually && current.IsManualSelectable())
			{
				float num3 = Vector3.Distance(current.mTransform.position, this.unit.mTransform.position);
				if (num3 < base.data.config.distance && current.buffManager != null && !current.buffManager.IsHaveBuff("Skill_Aier_03"))
				{
					if (num > num3)
					{
						units = current;
						num = num3;
					}
					if (Vector3.Dot(this.unit.mTransform.forward, current.mTransform.position - this.unit.mTransform.position) > 0f && num2 > num3)
					{
						num2 = num3;
						units2 = current;
					}
				}
			}
		}
		if (units2 != null)
		{
			return units2;
		}
		if (units != null)
		{
			return units;
		}
		return null;
	}

	public override bool NeedAutoLaunchToHero()
	{
		return false;
	}

	protected override void OnSkillPhase3End(SkillDataKey skill_key)
	{
		base.OnSkillPhase3End(skill_key);
	}

	protected override void OnSkillInterrupt()
	{
		base.OnSkillInterrupt();
	}

	public override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
	}
}
