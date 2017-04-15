using System;
using UnityEngine;

public class SkillUnits_AkaliInvisible : SkillUnit
{
	private void EnterInvisible()
	{
	}

	private void ExitInvisible()
	{
	}

	private float DistanceWithPoint()
	{
		if (base.trans == null)
		{
			return 0f;
		}
		if (base.ParentUnit == null)
		{
			return 0f;
		}
		return Vector3.Distance(base.ParentUnit.transform.position, base.trans.position);
	}

	public override void RemoveSelf(float delay = 0f)
	{
		this.isDestroy = true;
		this.ExitInvisible();
		base.RemoveSelf(delay);
	}

	public void RegisterEvent()
	{
	}

	private void ParentUnit_OnSkillCallback(Units arg1)
	{
		this.ExitInvisible();
	}

	private void ParentUnit_OnAttackCallback(Units arg1)
	{
		this.ExitInvisible();
	}
}
