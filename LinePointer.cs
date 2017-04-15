using System;
using UnityEngine;

public class LinePointer : SkillPointer
{
	[SerializeField]
	public Transform lineBottom;

	[SerializeField]
	public Transform lineTop;

	[SerializeField]
	private Transform m_root;

	[SerializeField]
	private SpriteRenderer m_arrowRenderer_top;

	[SerializeField]
	private SpriteRenderer m_arrowRenderer_bottom;

	[SerializeField]
	private Sprite m_spriteRectangleTopGreen;

	[SerializeField]
	private Sprite m_spriteRectangleTopYellow;

	[SerializeField]
	private Sprite m_spriteRectangleBottomGreen;

	[SerializeField]
	private Sprite m_spriteRectangelBottomYellow;

	private float m_fCurDistance;

	private float Length;

	public override void CreateSkillPointer(string skillMainId, float skillDistance, float effectRange1, float effectRange2)
	{
		base.CreateSkillPointer(skillMainId, skillDistance, effectRange1, effectRange2);
		this.m_AttackRange.gameObject.SetActive(false);
		Skill skillById = this.theurgist.getSkillById(skillMainId);
		base.transform.LookAt(this.theurgist.transform.position + this.theurgist.transform.forward * 2f);
		float x = effectRange1;
		float z = effectRange2;
		if (skillById.data.IsUseSkillPointer)
		{
			x = skillById.data.PointerWidth;
			z = skillById.data.PointerLength;
		}
		this.m_EffectiveRange.localScale = new Vector3(x, this.m_EffectiveRange.localScale.y, z);
		this.m_EffectiveRange.localPosition = Vector3.zero;
		this.m_EffectiveRange.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.m_EffectiveRange.name = "LinePointer";
	}

	public override void Show()
	{
		base.Show();
		this.m_EffectiveRange.gameObject.SetActive(true);
	}

	public override void ChangeTra(Vector3 point)
	{
		Vector3 worldPosition = point;
		if (this.theurgist != null)
		{
			worldPosition.y = this.theurgist.transform.position.y;
			base.transform.position = this.theurgist.transform.position;
		}
		base.transform.LookAt(worldPosition);
	}

	public override bool isInRange(Vector3 point)
	{
		this.m_fCurDistance = Vector3.Distance(this.endSelectObj.transform.position, point);
		return this.m_fCurDistance <= this.Length;
	}

	private void OutOfRange()
	{
		this.m_EffectiveRange.localPosition = new Vector3(0f, 0f, 0f);
		this.m_arrowRenderer_top.sprite = this.m_spriteRectangleTopYellow;
		this.m_arrowRenderer_bottom.sprite = this.m_spriteRectangelBottomYellow;
	}

	private void InTheRange()
	{
		this.m_EffectiveRange.localPosition = Vector3.zero;
		this.m_arrowRenderer_top.sprite = this.m_spriteRectangleTopGreen;
		this.m_arrowRenderer_bottom.sprite = this.m_spriteRectangleBottomGreen;
	}

	public override void AlphaChangeFalse()
	{
		this.m_arrowRenderer_top.color = new Color32(255, 255, 255, 255);
		this.m_arrowRenderer_bottom.color = new Color32(255, 255, 255, 255);
	}

	public override void AlphaChangeTrue()
	{
		this.m_arrowRenderer_top.color = new Color32(255, 255, 255, 60);
		this.m_arrowRenderer_bottom.color = new Color32(255, 255, 255, 60);
	}

	public void SetEffectRange(float inWidth, float inLength)
	{
		this.m_EffectiveRange.localScale = new Vector3(1f, 1f, inLength * 0.3f);
	}

	public void SetDirect(Vector3 inDirect)
	{
		this.m_EffectiveRange.forward = inDirect;
	}
}
