using System;
using UnityEngine;

public class SectorPointer : SkillPointer
{
	public enum SectorType
	{
		Angle_20 = 1,
		Angle_30,
		Angle_45,
		Angle_60
	}

	[SerializeField]
	private GameObject m_objSector20;

	[SerializeField]
	private GameObject m_objSector30;

	[SerializeField]
	private GameObject m_objSector45;

	[SerializeField]
	private GameObject m_objSector60;

	[SerializeField]
	private SpriteRenderer m_rendererSector20;

	[SerializeField]
	private SpriteRenderer m_rendererSector30;

	[SerializeField]
	private SpriteRenderer m_rendererSector45;

	[SerializeField]
	private SpriteRenderer m_rendererSector60;

	[SerializeField]
	private Sprite m_spriteSector20_Green;

	[SerializeField]
	private Sprite m_spriteSector20_Yellow;

	[SerializeField]
	private Sprite m_spriteSector30_Green;

	[SerializeField]
	private Sprite m_spriteSector30_Yellow;

	[SerializeField]
	private Sprite m_spriteSector45_Green;

	[SerializeField]
	private Sprite m_spriteSector45_Yellow;

	[SerializeField]
	private Sprite m_spriteSector60_Green;

	[SerializeField]
	private Sprite m_spriteSector60_Yellow;

	[SerializeField]
	private Material m_matSector;

	private GameObject m_curSectorObj;

	private SpriteRenderer m_curRenderer;

	private Sprite m_spriteGreen;

	private Sprite m_spriteYellow;

	private float m_fCurDistance;

	private float Radius;

	private int sectorType;

	public override void CreateSkillPointer(string skillMainId, float skillDistance, float effectRange1, float effectRange2)
	{
		base.CreateSkillPointer(skillMainId, skillDistance, effectRange1, effectRange2);
		base.transform.name = "SectorPointer";
		Skill skillById = this.theurgist.getSkillById(skillMainId);
		float num = skillDistance / 2f;
		if (skillById.data.IsUseSkillPointer)
		{
			num = skillById.Data.PointerRadius;
		}
		base.transform.LookAt(this.theurgist.transform.position + this.theurgist.transform.forward * 2f);
		this.m_AttackRange.localScale = new Vector3(num, this.m_AttackRange.localScale.y, num);
		this.m_AttackRange.localPosition = Vector3.zero;
		this.m_AttackRange.localRotation = Quaternion.Euler(default(Vector3));
	}

	public override void Show()
	{
		base.Show();
		this.m_AttackRange.gameObject.SetActive(true);
		this.m_AttackTweenScale.from = new Vector3(0f, 0f, 0f);
		this.m_AttackTweenScale.to = this.m_AttackRange.localScale;
		this.m_AttackTweenScale.Begin();
	}

	public override void ChangeTra(Vector3 mousePoint)
	{
		Vector3 worldPosition = mousePoint;
		if (this.theurgist != null)
		{
			worldPosition.y = this.theurgist.transform.position.y;
			base.transform.position = this.theurgist.transform.position;
		}
		base.transform.LookAt(worldPosition);
	}

	public override bool isInRange(Vector3 point)
	{
		this.m_fCurDistance = Vector3.Distance(this.theurgist.transform.position, point);
		return this.m_fCurDistance <= this.Radius;
	}

	private void OutOfRange()
	{
		this.m_curSectorObj.transform.localPosition = new Vector3(0f, 0f, Mathf.Abs(this.m_fCurDistance - this.Radius));
		if (!this.m_bIsChangeSprite)
		{
			this.m_curRenderer.sprite = this.m_spriteYellow;
			this.m_bIsChangeSprite = true;
		}
	}

	private void InTheRange()
	{
		if (this.m_bIsChangeSprite)
		{
			this.m_curSectorObj.transform.localPosition = Vector3.zero;
			this.m_curRenderer.sprite = this.m_spriteGreen;
			this.m_bIsChangeSprite = false;
		}
	}

	public override void AlphaChangeFalse()
	{
		this.m_curRenderer.color = new Color32(255, 255, 255, 255);
		this.m_curSectorObj.renderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
	}

	public override void AlphaChangeTrue()
	{
		this.m_curRenderer.color = new Color32(255, 255, 255, 60);
		this.m_curSectorObj.renderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
	}
}
