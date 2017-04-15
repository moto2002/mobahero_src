using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using UnityEngine;

public class Skill_Akali_04 : Skill_Timo_04
{
	public Skill_Akali_04(string skill_id, Units self) : base(skill_id, self)
	{
		base.NeedBuff = "buff_" + base.skillMainId;
		this.buffData = Singleton<BuffDataManager>.Instance.GetVo(base.NeedBuff);
		if (this.buffData == null)
		{
			Debug.LogError("BuffHostSkill buff is null: " + base.NeedBuff);
		}
	}

	protected override void ShowSkillActiveHint()
	{
	}

	protected override void HideSkillActiveHint()
	{
	}
}
