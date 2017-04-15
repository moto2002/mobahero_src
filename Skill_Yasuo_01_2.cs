using MobaFrame.SkillAction;
using System;
using UnityEngine;

public class Skill_Yasuo_01_2 : Skill_Yasuo_01
{
	private int skillIdx;

	public Skill_Yasuo_01_2()
	{
		this.skillIdx = 2;
	}

	public Skill_Yasuo_01_2(string skill_id, Units self) : base(skill_id, self)
	{
		this.skillIdx = 2;
	}

	public override void OnSkillReadyBegin(ReadySkillAction action)
	{
		base.OnSkillReadyBegin(action);
		if (this.skillIdx == 2)
		{
			Vector3? targetPosition = action.targetPosition;
			if (targetPosition.HasValue)
			{
				this.unit.mTransform.LookAt(action.targetPosition.Value);
			}
		}
	}
}
