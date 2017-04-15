using System;
using UnityEngine;

public class Skill_Yasuo_02 : Skill
{
	private const string unitTargetedBuff = "Skill_Aier_03";

	public Skill_Yasuo_02()
	{
	}

	public Skill_Yasuo_02(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override bool NeedResetTargetPos()
	{
		return true;
	}

	public override Vector3 GetExtraTargetPos(Units target, Vector3 targetPos, bool isCrazyMode = true)
	{
		if (target == null)
		{
			return targetPos;
		}
		float num = Vector3.Distance(this.unit.mTransform.position, target.mTransform.position);
		num /= 2f;
		if (num >= base.distance)
		{
			num = base.distance;
		}
		Vector3 a = target.mTransform.position - this.unit.mTransform.position;
		a.Normalize();
		return this.unit.mTransform.position + a * num;
	}
}
