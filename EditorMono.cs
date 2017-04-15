using System;
using UnityEngine;

public class EditorMono : MonoBehaviour
{
	private Transform cacheTrans;

	private GameObject cacheGO;

	public Transform Trans
	{
		get
		{
			return this.cacheTrans ?? base.transform;
		}
		set
		{
			this.cacheTrans = value;
		}
	}

	public GameObject GO
	{
		get
		{
			return this.cacheGO ?? base.gameObject;
		}
		set
		{
			this.cacheGO = value;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Update()
	{
	}
}
