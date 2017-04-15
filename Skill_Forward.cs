using System;
using UnityEngine;

public class Skill_Forward : Skill
{
	public Skill_Forward(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public Skill_Forward()
	{
	}

	public override bool NeedResetTargetPos()
	{
		return true;
	}

	public override Vector3 GetExtraTargetPos(Units target, Vector3 targetPos, bool isCrazyMode = true)
	{
		return this.unit.mTransform.position + this.unit.mTransform.forward * base.distance;
	}
}
