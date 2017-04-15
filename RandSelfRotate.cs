using System;
using UnityEngine;

public class RandSelfRotate : MonoBehaviour
{
	public Vector2 startXRotate = Vector2.zero;

	public Vector2 startYRotate = Vector2.zero;

	public Vector2 startZRotate = Vector2.zero;

	private float xRotate;

	private float yRotate;

	private float zRotate;

	public void OnSpawned()
	{
		this.xRotate = UnityEngine.Random.Range(this.startXRotate.x, this.startXRotate.y);
		this.yRotate = UnityEngine.Random.Range(this.startYRotate.x, this.startYRotate.y);
		this.zRotate = UnityEngine.Random.Range(this.startZRotate.x, this.startZRotate.y);
	}

	public void OnDespawned()
	{
	}

	private void Awake()
	{
		this.OnSpawned();
	}

	private void OnDestroy()
	{
		this.OnDespawned();
	}

	protected virtual void Update()
	{
		base.transform.Rotate(this.xRotate * Time.deltaTime, this.yRotate * Time.deltaTime, this.zRotate * Time.deltaTime);
	}
}
