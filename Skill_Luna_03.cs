using MobaFrame.SkillAction;
using MobaProtocol.Data;
using System;
using UnityEngine;

public class Skill_Luna_03 : Skill
{
	private Vector3 prePosition;

	private float curTime;

	private bool isStop = true;

	public Skill_Luna_03(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillReadyBegin(ReadySkillAction action)
	{
		base.OnSkillReadyBegin(action);
		this.prePosition = this.unit.trans.position;
		this.isStop = false;
	}

	public override void OnUpdate(float deltaTime)
	{
		if (!this.isStop)
		{
			Vector3 normalized = (this.unit.trans.position - this.prePosition).normalized;
			this.unit.trans.rotation = Quaternion.LookRotation(normalized, this.unit.trans.up);
		}
		this.curTime += deltaTime;
		base.OnUpdate(deltaTime);
		if (this.curTime >= 0.01f)
		{
			this.curTime = 0f;
			this.prePosition = this.unit.trans.position;
		}
	}

	public override void SynInfo(SynSkillInfo info)
	{
		base.SynInfo(info);
		this.isStop = true;
	}
}
