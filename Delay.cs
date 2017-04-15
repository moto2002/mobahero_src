using System;
using UnityEngine;

public class Delay : MonoBehaviour
{
	public float delayTime = 1f;

	private void Start()
	{
		base.gameObject.SetActive(false);
		base.Invoke("DelayFunc", this.delayTime);
	}

	private void DelayFunc()
	{
		base.gameObject.SetActive(true);
	}
}
