using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;

public class Skill_Huimiezhe_04 : Skill
{
	public Skill_Huimiezhe_04(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillStartBegin(StartSkillAction action)
	{
		base.OnSkillStartBegin(action);
		for (int i = 0; i < 7; i++)
		{
			if (i != 2)
			{
				if (this.unit.isPlayer)
				{
					Singleton<SkillView>.Instance.SetActiveSkill(i, false);
					Singleton<SkillView>.Instance.SetSkillUIForbidMask(i, true);
				}
			}
		}
	}

	public override void SynInfo(SynSkillInfo info)
	{
		base.SynInfo(info);
		this.unit.ForceIdle();
		for (int i = 0; i < 7; i++)
		{
			if (i != 2)
			{
				if (this.unit.isPlayer)
				{
					Singleton<SkillView>.Instance.SetActiveSkill(i, true);
					Singleton<SkillView>.Instance.SetSkillUIForbidMask(i, false);
				}
			}
		}
		GlobalObject.Instance.StartCoroutine(this.DelayRefreshIcon());
	}

	[DebuggerHidden]
	private IEnumerator DelayRefreshIcon()
	{
		Skill_Huimiezhe_04.<DelayRefreshIcon>c__Iterator9C <DelayRefreshIcon>c__Iterator9C = new Skill_Huimiezhe_04.<DelayRefreshIcon>c__Iterator9C();
		<DelayRefreshIcon>c__Iterator9C.<>f__this = this;
		return <DelayRefreshIcon>c__Iterator9C;
	}

	protected override void OnSkillInterrupt()
	{
		base.OnSkillInterrupt();
		for (int i = 0; i < 7; i++)
		{
			if (i != 2)
			{
				if (this.unit.isPlayer)
				{
				}
			}
		}
	}
}
