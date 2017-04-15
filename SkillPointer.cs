using System;
using UnityEngine;

public abstract class SkillPointer : MonoBehaviour
{
	public float angel;

	public Vector3 pos;

	public bool isShow;

	public bool isCancel;

	public bool MoveOutSkillUI;

	public bool AplhaChange;

	public GameObject endSelectObj;

	public Units endSelectUnit;

	public bool isConjuring;

	public bool m_bNeedRangePointer;

	public float m_fMaxRange = 5f;

	public float m_fMinRange;

	public bool m_bIsChangeSprite;

	public bool isReady;

	public Units theurgist;

	public Transform target;

	public Transform m_AttackRange;

	public Transform m_EffectiveRange;

	[SerializeField]
	protected TweenScale m_AttackTweenScale;

	[SerializeField]
	protected TweenScale m_EffectTweenScale;

	public Vector3 endPosition;

	public string skillMainId;

	protected float skillDistance;

	public EffectFollowBehavior followBehavior;

	private void Start()
	{
		this.isCancel = false;
	}

	private void OnDestroy()
	{
		this.isReady = false;
		this.isShow = false;
	}

	public virtual void CreateSkillPointer(string skillMainId, float skillDistance, float effectRange1, float effectRange2)
	{
		this.skillMainId = skillMainId;
		this.skillDistance = skillDistance;
		this.endPosition = base.transform.position;
		this.isReady = false;
		base.gameObject.SetActive(false);
	}

	public virtual void Show()
	{
		base.gameObject.SetActive(true);
		this.isShow = true;
		this.isReady = false;
	}

	public abstract void ChangeTra(Vector3 point);

	public abstract bool isInRange(Vector3 point);

	public abstract void AlphaChangeTrue();

	public abstract void AlphaChangeFalse();

	public virtual void DestroySelf()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public virtual void Hide()
	{
		base.gameObject.SetActive(false);
		this.m_AttackRange.gameObject.SetActive(false);
		this.isShow = false;
		this.isReady = false;
	}

	public void SetPosition(Vector3 inPosition)
	{
		Vector3 offset = inPosition - base.transform.position;
		this.followBehavior.offset = offset;
		base.transform.position = inPosition;
	}

	public void SetAttackRange(float inRadius)
	{
		this.m_AttackRange.localScale = new Vector3(inRadius, 1f, inRadius);
	}
}
