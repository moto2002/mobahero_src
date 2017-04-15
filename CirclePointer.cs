using System;
using UnityEngine;

public class CirclePointer : SkillPointer
{
	[SerializeField]
	private Sprite m_spriteCircleGreen;

	[SerializeField]
	private Sprite m_spriteCircleYellow;

	private float Radius;

	private float Z_Distance;

	public override void CreateSkillPointer(string skillMainId, float skillDistance, float effectRange1, float effectRange2)
	{
		base.CreateSkillPointer(skillMainId, skillDistance, effectRange1, effectRange2);
		base.transform.name = "CirclePointer";
		Skill skillById = this.theurgist.getSkillById(skillMainId);
		float num = skillDistance;
		if (skillById.data.IsUseSkillPointer)
		{
			num = skillById.data.PointerRadius;
		}
		this.m_AttackRange.localScale = new Vector3(num, this.m_AttackRange.localScale.y, num);
		this.m_AttackRange.localPosition = Vector3.zero;
		this.m_AttackRange.localRotation = Quaternion.Euler(Vector3.zero);
	}

	public override void Show()
	{
		base.Show();
		this.m_AttackRange.gameObject.SetActive(true);
		this.m_AttackTweenScale.from = new Vector3(0f, 0f, 0f);
		this.m_AttackTweenScale.to = this.m_AttackRange.localScale;
		this.m_AttackTweenScale.Begin();
	}

	public override void ChangeTra(Vector3 point)
	{
	}

	public override bool isInRange(Vector3 point)
	{
		float num = Vector3.Distance(this.theurgist.transform.position, point);
		return num <= this.Radius;
	}

	private void OutOfRange()
	{
		if (!this.m_bIsChangeSprite)
		{
			this.m_bIsChangeSprite = true;
		}
	}

	private void InTheRange()
	{
		if (this.m_bIsChangeSprite)
		{
			this.m_bIsChangeSprite = false;
		}
	}

	public override void AlphaChangeFalse()
	{
	}

	public override void AlphaChangeTrue()
	{
	}

	public void SetEffectRange(float inRadius)
	{
		this.m_EffectiveRange.localScale = Vector3.one * this.Radius;
	}
}
