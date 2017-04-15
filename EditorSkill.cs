using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EditorSkill
{
	private SkillData data;

	private EditorUnit unit;

	private string skillid;

	private SkillDataKey skillKey;

	private List<EditorUnit> targets;

	private Vector3? skillPoint;

	public void Init(EditorUnit unit, string _skillId)
	{
		this.skillid = _skillId;
		this.unit = unit;
		this.data = new SkillData(_skillId, 1, 0);
		this.skillKey = new SkillDataKey(this.skillid, 1, 0);
	}

	public void Start(List<EditorUnit> targets, Vector3? skillPoint)
	{
		this.targets = targets;
		this.skillPoint = skillPoint;
		this.unit.StartCoroutine(this.skill_enumator());
	}

	[DebuggerHidden]
	private IEnumerator skill_enumator()
	{
		EditorSkill.<skill_enumator>c__Iterator1E3 <skill_enumator>c__Iterator1E = new EditorSkill.<skill_enumator>c__Iterator1E3();
		<skill_enumator>c__Iterator1E.<>f__this = this;
		return <skill_enumator>c__Iterator1E;
	}

	private void StartBuffs(string[] buffs, List<EditorUnit> targets)
	{
		if (buffs == null)
		{
			return;
		}
		if (targets == null)
		{
			return;
		}
		for (int i = 0; i < buffs.Length; i++)
		{
			for (int j = 0; j < targets.Count; j++)
			{
				targets[j].BuffManager.Add(buffs[i], this);
			}
		}
	}

	public static List<EditorPerform> StartPerform(string[] performs, EditorUnit unit, List<EditorUnit> targets, Vector3? skillPoint, EditorSkill skill = null)
	{
		if (performs == null)
		{
			return null;
		}
		List<EditorPerform> list = new List<EditorPerform>();
		for (int i = 0; i < performs.Length; i++)
		{
			PerformData vo = Singleton<PerformDataManager>.Instance.GetVo(performs[i]);
			EditorPerform editorPerform;
			if (vo.effect_type == PerformType.Normal || vo.effect_type == PerformType.Hit)
			{
				editorPerform = new EditorPerform();
			}
			else if (vo.effect_type == PerformType.Missile || vo.effect_type == PerformType.Parabola || vo.effect_type == PerformType.MissileBomb)
			{
				editorPerform = new EditorMissilePerform();
			}
			else if (vo.effect_type == PerformType.Bullet)
			{
				editorPerform = new EditorBulletPerform();
			}
			else if (vo.effect_type == PerformType.Link)
			{
				editorPerform = new EditorLinkPerform();
			}
			else if (vo.effect_type == PerformType.Darts)
			{
				editorPerform = new EditorDartsPerform();
			}
			else
			{
				editorPerform = new EditorPerform();
			}
			editorPerform.Start(performs[i], unit, targets, skillPoint, skill);
			list.Add(editorPerform);
		}
		return list;
	}

	public static void RemovePerform(List<EditorPerform> performs)
	{
		if (performs == null)
		{
			return;
		}
		for (int i = 0; i < performs.Count; i++)
		{
			performs[i].doDestroy();
		}
	}

	private void StartHitPerform(string[] performs, List<EditorUnit> targets, Vector3? skillPoint)
	{
		if (performs == null)
		{
			return;
		}
		for (int i = 0; i < performs.Length; i++)
		{
			for (int j = 0; j < targets.Count; j++)
			{
				EditorPerform editorPerform = new EditorPerform();
				editorPerform.Start(performs[i], targets[j], null, skillPoint, this);
			}
		}
	}

	public void OnHit(List<EditorUnit> targets)
	{
		this.StartHitPerform(this.data.hit_actions, targets, this.skillPoint);
		this.StartBuffs(this.data.hit_buff_ids, targets);
	}
}
