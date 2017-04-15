using System;
using UnityEngine;

public class DelayAction : MonoBehaviour
{
	public float delayTime = 4f;

	private void OnEnable()
	{
		base.Invoke("DoAction", this.delayTime);
	}

	private void OnDisable()
	{
		base.CancelInvoke("DoAction");
	}

	private void DoAction()
	{
		base.gameObject.SetActive(false);
	}
}
