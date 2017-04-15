using System;
using UnityEngine;

public class FXDKZSTOU : MonoBehaviour
{
	public float scal = 0.9f;

	public Vector3 offset = new Vector3(0f, 0.01f, 0f);

	private Transform trans;

	private void Start()
	{
		this.trans = base.transform;
	}

	private void Update()
	{
		this.trans.localScale = Vector3.one * this.scal;
		this.trans.localPosition = Vector3.zero + this.offset;
		this.trans.localRotation = Quaternion.identity;
	}
}
