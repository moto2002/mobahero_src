using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorSkillManager : EditorMono, IEditorUnitCompoent
{
	private EditorUnit self;

	public List<EditorSkill> attack = new List<EditorSkill>();

	public List<EditorSkill> skill = new List<EditorSkill>();

	private SysHeroMainVo data;

	private int curAttackIndex;

	public void Attack(EditorUnit targetUnit)
	{
		if (this.curAttackIndex >= this.attack.Count)
		{
			this.curAttackIndex = 0;
		}
		this.attack[this.curAttackIndex].Start(new List<EditorUnit>
		{
			targetUnit
		}, null);
		this.curAttackIndex++;
		this.self.MoveController.Stop();
		this.self.MoveController.LookAt(targetUnit.Trans.position);
	}

	public void Skill(int index, EditorUnit targetUnit, Vector3? pos)
	{
		if (index > this.skill.Count)
		{
			return;
		}
		this.self.MoveController.Stop();
		this.skill[index].Start(new List<EditorUnit>
		{
			targetUnit
		}, pos);
	}

	public void Init(EditorUnit unit)
	{
		this.self = unit;
		if (this.self.Unit is Hero)
		{
			this.data = BaseDataMgr.instance.GetHeroMainData(this.self.Unit.npc_id);
			if (this.data != null)
			{
				this.InitAttacks();
				this.InitSkills();
			}
		}
	}

	private void InitAttacks()
	{
		string[] array = StringUtils.SplitVoString(this.data.attack_id, ",");
		for (int i = 0; i < array.Length; i++)
		{
			EditorSkill editorSkill = new EditorSkill();
			editorSkill.Init(this.self, array[i]);
			this.attack.Add(editorSkill);
		}
	}

	private void InitSkills()
	{
		string[] array = StringUtils.SplitVoString(this.data.skill_id, ",");
		for (int i = 0; i < array.Length; i++)
		{
			EditorSkill editorSkill = new EditorSkill();
			editorSkill.Init(this.self, array[i]);
			this.skill.Add(editorSkill);
		}
	}
}
