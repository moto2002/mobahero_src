using System;
using UnityEngine;

public class CircleSelectorPointer : SkillPointer
{
	private Skill _theSkill;

	public override void CreateSkillPointer(string skillMainId, float skillDistance, float effectRange1, float effectRange2)
	{
		base.CreateSkillPointer(skillMainId, skillDistance, effectRange1, effectRange2);
		base.transform.name = "CircleSelectorPointer";
		this._theSkill = this.theurgist.getSkillById(skillMainId);
		this.m_AttackRange.localScale = new Vector3(skillDistance, this.m_AttackRange.localScale.y, skillDistance);
		this.m_EffectiveRange.localScale = new Vector3(effectRange1, this.m_EffectiveRange.localScale.y, effectRange1);
		this.m_AttackRange.localPosition = Vector3.zero;
		this.m_EffectiveRange.localPosition = Vector3.zero;
		this.m_AttackRange.localRotation = Quaternion.Euler(Vector3.zero);
		this.m_EffectiveRange.localRotation = Quaternion.Euler(Vector3.zero);
	}

	public override void Hide()
	{
		base.gameObject.SetActive(false);
		this.isShow = false;
		this.isReady = false;
	}

	public override void Show()
	{
		base.gameObject.SetActive(true);
		this.isShow = true;
		this.isReady = true;
	}

	public override bool isInRange(Vector3 point)
	{
		Debug.LogError("Runned Circle Selector");
		return true;
	}

	public override void ChangeTra(Vector3 point)
	{
		Vector3 position = point;
		position.y = this.theurgist.transform.position.y;
		if (this.theurgist != null && this._theSkill != null)
		{
			base.transform.position = this.theurgist.transform.position;
			if ((point - base.transform.position).sqrMagnitude >= this._theSkill.distance * this._theSkill.distance)
			{
				position = base.transform.position + (point - base.transform.position).normalized * this._theSkill.distance;
			}
			else
			{
				position = point;
			}
		}
		else
		{
			position = base.transform.position;
		}
		this.m_EffectiveRange.position = position;
	}

	public override void AlphaChangeFalse()
	{
		Debug.LogError("Runned Circle Selector");
	}

	public override void AlphaChangeTrue()
	{
		Debug.LogError("Runned Circle Selector");
	}
}
