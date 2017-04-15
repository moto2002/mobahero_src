using System;
using UnityEngine;

public class EffectFollowBehavior : MonoBehaviour
{
	[SerializeField]
	public Transform centerObject;

	[SerializeField]
	public Vector3 offset = Vector3.zero;

	[SerializeField]
	public bool isActive;

	protected virtual void Awake()
	{
		this.isActive = false;
		this.offset = Vector3.zero;
	}

	protected virtual void Update()
	{
		if (!this.isActive || this.centerObject == null)
		{
			return;
		}
		base.transform.position = this.centerObject.position + this.offset;
	}

	public void SetFollowObj(Transform target, Vector3 offset)
	{
		this.centerObject = target;
		this.offset = offset;
		if (this.centerObject != null)
		{
			base.transform.position = this.centerObject.position + offset;
			this.isActive = true;
		}
	}

	protected virtual void OnSpawned()
	{
		this.isActive = false;
		this.offset = Vector3.zero;
	}

	protected virtual void OnDespawned()
	{
		this.isActive = false;
		this.offset = Vector3.zero;
	}
}
