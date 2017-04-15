using MobaFrame.SkillAction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Tunvlang_01 : Skill
{
	public struct castTime
	{
		public float before;

		public float @in;

		public float end;

		public castTime(float _before, float _in, float _end)
		{
			this.before = _before;
			this.@in = _in;
			this.end = _end;
		}
	}

	private int totalCount;

	private float intervalTime = 3f;

	private float curTime;

	private bool isUpdateTime;

	private int curConjIndex;

	private int preConjIndex;

	public Skill_Tunvlang_01.castTime[] _castTime = new Skill_Tunvlang_01.castTime[3];

	public Skill_Tunvlang_01()
	{
	}

	public Skill_Tunvlang_01(string skill_id, Units self) : base(skill_id, self)
	{
		this.RevertTotalCount();
		this._castTime[0].before = 0f;
		this._castTime[0].@in = 0.3f;
		this._castTime[0].end = 0f;
		this._castTime[1].before = 0f;
		this._castTime[1].@in = 0.3f;
		this._castTime[1].end = 0f;
		this._castTime[2].before = 0f;
		this._castTime[2].@in = 0.45f;
		this._castTime[2].end = 0f;
	}

	public override bool CanInterrupt(SkillInterruptType type)
	{
		return (!base.IsAttack && type == SkillInterruptType.Passive) || base.CanInterrupt(type);
	}

	protected override void OnSkillInterrupt()
	{
		base.OnSkillInterrupt();
		this.unit.skillManager.DestroyActions();
		this.unit.highEffManager.stopHighEffeCoroutine();
		LSDebug.log("skillinterrupted!");
	}

	protected override void OnSkillStart()
	{
		base.OnSkillStart();
		base.data.castBefore = this._castTime[this.GetCurConjureIndex()].before;
		base.data.castIn = this._castTime[this.GetCurConjureIndex()].@in;
		base.data.castEnd = this._castTime[this.GetCurConjureIndex()].end;
	}

	public override void genericSet()
	{
		this.RevertUpdateTime();
		if (!this.IsCountOut())
		{
			this.isUpdateTime = true;
			return;
		}
	}

	public void RemoveCount()
	{
		this.totalCount--;
		this.preConjIndex = this.curConjIndex;
		this.curConjIndex++;
	}

	public void RevertTotalCount()
	{
		this.totalCount = 2;
		this.preConjIndex = this.curConjIndex;
		this.curConjIndex = 0;
	}

	private void RevertUpdateTime()
	{
		this.curTime = 0f;
		this.isUpdateTime = false;
	}

	private void UpdateTime(float deltaTime)
	{
		if (!this.isUpdateTime)
		{
			return;
		}
		this.curTime += deltaTime;
		if (this.curTime > this.intervalTime && !base.IsInSkillCastBefore)
		{
			this.totalCount = 0;
			this.RevertTotalCount();
			this.RevertUpdateTime();
		}
	}

	public override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
		this.UpdateTime(deltaTime);
	}

	public int GetCurConjureIndex()
	{
		return Mathf.Clamp(this.curConjIndex, 0, 2);
	}

	public int GetPreConjureIndex()
	{
		return this.preConjIndex;
	}

	public bool IsCountOut()
	{
		return this.totalCount <= 0;
	}

	protected override void AddHighEff(SkillDataKey skill_key, SkillPhrase skillPhrase, List<Units> targets = null, Vector3? skillPosition = null)
	{
		if (!StringUtils.CheckValid(skill_key.SkillID))
		{
			return;
		}
		SkillData data = GameManager.Instance.SkillData.GetData(skill_key);
		string[] highEffects = data.GetHighEffects(skillPhrase);
		if (highEffects != null)
		{
			int curConjureIndex = this.GetCurConjureIndex();
			if (highEffects.Length > curConjureIndex)
			{
				string text = highEffects[curConjureIndex];
				switch (skillPhrase)
				{
				case SkillPhrase.Start:
				case SkillPhrase.Init:
					if (!SkillUtility.IsImmunityHighEff(this.unit, text))
					{
						ActionManager.AddHighEffect(text, data.skillId, this.unit, this.unit, skillPosition, true);
					}
					break;
				case SkillPhrase.Hit:
					for (int i = 0; i < targets.Count; i++)
					{
						if (targets[i] != null && targets[i].isLive && !SkillUtility.IsImmunityHighEff(targets[i], text))
						{
							ActionManager.AddHighEffect(text, data.skillId, targets[i], this.unit, skillPosition, true);
						}
					}
					break;
				}
			}
		}
	}

	public override void StartCDTime(float cd = -1f, bool isReset = false)
	{
		this.isUpdateTime = true;
		this.curTime = 0f;
		base.StartCDTime(cd, false);
	}
}
