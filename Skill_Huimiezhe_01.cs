using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;

public class Skill_Huimiezhe_01 : Skill
{
	public Skill_Huimiezhe_01(string skill_id, Units self) : base(skill_id, self)
	{
	}

	protected override void OnSkillInterrupt()
	{
		base.OnSkillInterrupt();
		if (!this.unit.YunXuan.IsInState)
		{
			this.unit.ForceIdle();
		}
		AudioMgr.Play("Stop_Huimiezhe_1_Loop", this.unit.gameObject, false, false);
	}

	public override void SynInfo(SynSkillInfo info)
	{
		base.SynInfo(info);
		GlobalObject.Instance.StartCoroutine(this.DelyaIdel());
	}

	[DebuggerHidden]
	private IEnumerator DelyaIdel()
	{
		Skill_Huimiezhe_01.<DelyaIdel>c__Iterator9B <DelyaIdel>c__Iterator9B = new Skill_Huimiezhe_01.<DelyaIdel>c__Iterator9B();
		<DelyaIdel>c__Iterator9B.<>f__this = this;
		return <DelyaIdel>c__Iterator9B;
	}
}
