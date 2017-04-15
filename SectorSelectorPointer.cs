using System;
using UnityEngine;

public class SectorSelectorPointer : SkillPointer
{
	private Skill _theSkill;

	public override void CreateSkillPointer(string skillMainId, float skillDistance, float effectRange1, float effectRange2)
	{
		base.CreateSkillPointer(skillMainId, skillDistance, effectRange1, effectRange2);
		base.transform.name = "SectorSelectorPointer";
		this._theSkill = this.theurgist.getSkillById(skillMainId);
		base.transform.localScale = new Vector3(skillDistance, base.transform.localScale.y, skillDistance);
		this.m_EffectiveRange.localPosition = Vector3.zero;
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
		Debug.LogError("Runned Sector Selector");
		return true;
	}

	public override void ChangeTra(Vector3 point)
	{
		if (this.theurgist != null)
		{
			point.y = this.theurgist.transform.position.y;
			base.transform.position = this.theurgist.transform.position;
		}
		base.transform.LookAt(point);
	}

	public override void AlphaChangeFalse()
	{
		Debug.LogError("Runned Sector Selector");
	}

	public override void AlphaChangeTrue()
	{
		Debug.LogError("Runned Sector Selector");
	}
}
