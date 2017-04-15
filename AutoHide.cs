using System;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
	public bool destroySelf;

	public float delay = 10f;

	private void OnEnable()
	{
		base.Invoke("HideSelf", this.delay);
	}

	private void HideSelf()
	{
		if (this.destroySelf)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
