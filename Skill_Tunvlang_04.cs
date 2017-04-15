using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class Skill_Tunvlang_04 : Skill
{
	private int count;

	private float intervalTime = 10f;

	private float curTime;

	private bool isUpdateTime;

	private Skill[] subSkill = new Skill[2];

	public override List<SkillData> SkillDataList
	{
		get
		{
			if (this.subSkill[this.count] == null)
			{
				return base.SkillDataList;
			}
			return this.subSkill[this.count].SkillDataList;
		}
	}

	public override SkillDataKey skillKey
	{
		get
		{
			return new SkillDataKey(this.subSkill[this.count].skillMainId, this.skillLevel, 0);
		}
	}

	public Skill_Tunvlang_04()
	{
	}

	public Skill_Tunvlang_04(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void DoSkillLevelUp()
	{
		this.subSkill[0] = this.self.getSkillById("Skill_Tunvlang_04_1");
		this.subSkill[1] = this.self.getSkillById("Skill_Tunvlang_04_2");
		base.DoSkillLevelUp();
	}

	public override void SynInfo(SynSkillInfo info)
	{
		this.count = info.extraInt1;
		this.Refresh();
	}

	private void Refresh()
	{
		if (this.count == 0)
		{
			if (this.unit.isPlayer)
			{
				Singleton<SkillView>.Instance.HideTriggerBornPowerObjHint(base.skillMainId);
			}
		}
		else if (this.unit.isPlayer)
		{
			Singleton<SkillView>.Instance.ShowTriggerBornPowerObjHint(base.skillMainId);
		}
	}

	private void Switch()
	{
		if (this.count == 0)
		{
			this.isUpdateTime = true;
			this.count = 1;
		}
		else
		{
			this.count = 0;
		}
		this.Refresh();
	}

	protected override void OnSkillPhase3End(SkillDataKey skill_key)
	{
		base.OnSkillPhase3End(skill_key);
		this.Switch();
	}

	protected override void OnSkillInterrupt()
	{
		base.OnSkillInterrupt();
		this.Switch();
	}

	public override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
		this.UpdateTime(deltaTime);
	}

	private void UpdateTime(float deltaTime)
	{
		if (!this.isUpdateTime)
		{
			return;
		}
		this.curTime += deltaTime;
		if (this.curTime > this.intervalTime)
		{
			base.genericSet();
			this.isUpdateTime = false;
			this.curTime = 0f;
			this.count = 0;
			this.Refresh();
		}
	}
}
