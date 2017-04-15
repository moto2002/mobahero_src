using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class Skill_Yasuo_01 : Skill
{
	private float intervalTime = 10f;

	private float curTime;

	private bool isUpdateTime;

	private Skill[] subSkill = new Skill[5];

	public override List<SkillData> SkillDataList
	{
		get
		{
			Skill skill = this.subSkill[this.skillSubIdx];
			if (skill == null || this.skillSubIdx == 0)
			{
				return base.SkillDataList;
			}
			return this.subSkill[this.skillSubIdx].SkillDataList;
		}
	}

	public override string SkillIcon
	{
		get
		{
			return base.Data.config.skill_icon;
		}
	}

	public override SkillDataKey skillKey
	{
		get
		{
			if (this.subSkill == null)
			{
				return default(SkillDataKey);
			}
			return new SkillDataKey(this.subSkill[this.skillSubIdx].skillMainId, this.skillLevel, 0);
		}
	}

	public Skill_Yasuo_01()
	{
	}

	public Skill_Yasuo_01(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void DoSkillLevelUp()
	{
		this.subSkill[0] = this.self.getSkillById("Skill_Aier_01");
		this.subSkill[1] = this.self.getSkillById("Skill_Aier_01_1");
		this.subSkill[2] = this.self.getSkillById("Skill_Aier_01_2");
		this.subSkill[3] = this.self.getSkillById("Skill_Aier_01_3");
		this.subSkill[4] = this.self.getSkillById("Skill_Aier_01_4");
		base.DoSkillLevelUp();
	}

	public override void SynInfo(SynSkillInfo info)
	{
		this.skillSubIdx = info.extraInt1;
		if (this.skillSubIdx == 2)
		{
			this.RefreshSkillIcon();
		}
		else
		{
			this.RefreshSkillIcon();
		}
	}

	public override void RefreshSkillIcon()
	{
		if (this.unit.isPlayer)
		{
			if (this.skillSubIdx == 2)
			{
				Singleton<SkillView>.Instance.RefreshIcon(0, this.subSkill[2].SkillIcon);
			}
			else
			{
				Singleton<SkillView>.Instance.RefreshIcon(0, this.subSkill[0].SkillIcon);
			}
		}
	}

	public override void OnSkillReadyBegin(ReadySkillAction action)
	{
		base.OnSkillReadyBegin(action);
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
