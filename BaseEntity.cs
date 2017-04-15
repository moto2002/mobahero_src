using System;
using UnityEngine;

public abstract class BaseEntity : MobaMono
{
	public static bool IsRefresh = true;

	[NonSerialized]
	public int unique_id;

	[NonSerialized]
	public string npc_id;

	[NonSerialized]
	public string summonerName;

	[NonSerialized]
	public string summonerId;

	protected float updateTime = 0.5f;

	protected bool isLateUpdate = true;

	protected bool isGameOver;

	protected bool isGameExit;

	protected bool isCreate;

	protected bool isStart;

	protected bool isDestroy;

	public bool isVisible = true;

	private Transform m_transform;

	public Transform mTransform
	{
		get
		{
			if (this.m_transform == null)
			{
				if (this == null)
				{
					return null;
				}
				this.m_transform = base.transform;
			}
			return this.m_transform;
		}
	}

	private void OnSpawned()
	{
		this.UnitCreate();
	}

	private void OnDespawned()
	{
		this.UnitStop();
	}

	protected virtual void Awake()
	{
		this.UnitCreate();
	}

	protected virtual void OnDestroy()
	{
		this.UnitDestroy();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!BaseEntity.IsRefresh)
		{
			return;
		}
		if (GameManager.IsPlaying())
		{
			if (this.isLateUpdate)
			{
				this.OnUpdate(Time.deltaTime);
			}
		}
		else if (GameManager.IsGameOver())
		{
			if (!this.isGameOver)
			{
				this.OnEnd();
				this.isGameOver = true;
			}
		}
		else if (GameManager.IsGameExit() && !this.isGameExit)
		{
			this.OnExit();
			this.isDestroy = true;
			this.isGameExit = true;
		}
	}

	protected virtual void OnCreate()
	{
	}

	protected virtual void OnInit(bool isRebirth = false)
	{
	}

	protected virtual void OnStart()
	{
	}

	protected virtual void OnStop()
	{
	}

	protected virtual void OnUpdate(float delta)
	{
	}

	protected virtual void OnEnd()
	{
	}

	protected virtual void OnExit()
	{
	}

	public void UnitCreate()
	{
		if (!this.isCreate)
		{
			this.OnCreate();
			this.isCreate = true;
			this.isDestroy = false;
		}
		this.isGameExit = false;
		this.isGameOver = false;
	}

	public void UnitInit(bool isRebirth = false)
	{
		this.OnInit(isRebirth);
	}

	public void UnitStart()
	{
		this.OnStart();
	}

	public void UnitStop()
	{
		if (!GameManager.IsGameExit())
		{
			this.OnStop();
		}
		this.isStart = false;
	}

	public void UnitDestroy()
	{
		if (!this.isDestroy)
		{
			this.OnExit();
			this.isDestroy = true;
		}
		this.isCreate = false;
		this.isStart = false;
	}
}
