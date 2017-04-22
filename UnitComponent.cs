using System;
using UnityEngine;

public class UnitComponent : UComponent<Units>
{
    /// <summary>
    /// 怪物是否执行update更新
    /// </summary>
	public bool donotUpdateByMonster;
    /// <summary>
    /// unit组件协程管理器
    /// </summary>
	protected CoroutineManager m_CoroutineManager = new CoroutineManager();

	private GameObject m_gameObject;

	private Transform m_transform;

	private CapsuleCollider m_collider;

	public GameObject gameObject
	{
		get
		{
			if (this.m_gameObject == null && this.self != null)
			{
				this.m_gameObject = this.self.gameObject;
			}
			return this.m_gameObject;
		}
	}

	public Transform transform
	{
		get
		{
			if (this.m_transform == null && this.self != null)
			{
				this.m_transform = this.self.mTransform;
			}
			return this.m_transform;
		}
	}

	public CapsuleCollider collider
	{
		get
		{
			if (this.self != null)
			{
				this.m_collider = this.self.mCollider;
			}
			return this.m_collider;
		}
	}

	public UnitComponent()
	{
	}

	public UnitComponent(Units self)
	{
		this.self = self;
	}
    /// <summary>
    /// 需要update更新
    /// </summary>
    /// <returns></returns>
	public virtual bool needUpdate()
	{
		return true;
	}
}
