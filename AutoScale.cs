using System;
using UnityEngine;

public class AutoScale : MonoBehaviour
{
	public float time = 4f;

	private float curTime;

	private bool isActive;

	private void Start()
	{
	}

	private void Update()
	{
		if (!this.isActive)
		{
			return;
		}
		if (this.curTime > this.time)
		{
			this.isActive = false;
			base.transform.localScale = Vector3.one * 0.01f;
		}
		this.curTime += Time.deltaTime;
	}

	private void OnEnable()
	{
		this.isActive = true;
		this.curTime = 0f;
		base.transform.localScale = Vector3.one;
	}

	private void OnDisable()
	{
		this.isActive = false;
		this.curTime = 0f;
	}
}
