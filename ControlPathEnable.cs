using System;
using UnityEngine;

public class ControlPathEnable : MonoBehaviour
{
	public float delayTime;

	public float intervalTime;

	private void Start()
	{
		base.InvokeRepeating("Repeat", this.delayTime, this.intervalTime);
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
		base.CancelInvoke("Repeat");
	}

	private void Repeat()
	{
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
	}
}
