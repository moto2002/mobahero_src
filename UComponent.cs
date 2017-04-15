using System;
using UnityEngine;

public abstract class UComponent<T>
{
	[SerializeField]
	protected T self;

	public void Create(T u)
	{
		this.self = u;
		this.OnCreate();
	}

	public virtual void OnCreate()
	{
	}

	public virtual void OnInit()
	{
	}

	public virtual void OnStart()
	{
	}

	public virtual void OnStop()
	{
	}

	public virtual void OnUpdate(float deltaTime)
	{
	}

	public virtual void OnExit()
	{
	}

	public virtual void OnDeath(T attacker)
	{
	}

	public virtual void OnTarget(T attacker)
	{
	}

	public virtual void OnHit(T attacker)
	{
	}

	public virtual void OnWound(T attacker, float damage)
	{
	}

	public virtual void OnLevelUp(int level)
	{
	}
}
