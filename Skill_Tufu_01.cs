using MobaFrame.SkillAction;
using MobaProtocol.Data;
using System;

public class Skill_Tufu_01 : Skill
{
	private Hook hook;

	public Skill_Tufu_01()
	{
	}

	public Skill_Tufu_01(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillHitBegin(HitSkillAction action)
	{
		base.OnSkillHitBegin(action);
		this.OnHookBack();
	}

	public override void OnPerFormeCreate(BasePerformAction obj)
	{
		if (obj != null)
		{
			this.hook = (obj as Hook);
			if (this.hook != null)
			{
				this.hook.Callback_OnHookBack = new Callback(this.OnHookBack);
				this.hook.Callback_OnDestroy = new Callback(this.OnHookDestroy);
			}
		}
	}

	public override void OnHighEffStart(BaseHighEffAction highEff)
	{
		if (highEff != null)
		{
			highEff.Callback_OnDestroy = new Callback(this.OnHookBackDestroy);
		}
	}

	private void OnHookBack()
	{
		if (this.unit.animController.GetMecanim().animator.GetCurrentAnimatorStateInfo(0).IsName("conjure1_2"))
		{
			this.unit.animController.PlayAnim(AnimationType.Conjure, true, 6, true, false);
		}
	}

	private void OnHookDestroy()
	{
		this.unit.animController.ForceIdle();
	}

	private void OnHookBackDestroy()
	{
		if (this.hook != null)
		{
			this.hook.Destroy();
		}
	}

	public override void SynInfo(SynSkillInfo info)
	{
		base.SynInfo(info);
		this.OnHookBackDestroy();
		this.OnHookDestroy();
	}
}
