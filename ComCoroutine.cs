using System;
using System.Collections;
using UnityEngine;

public class ComCoroutine : MonoBehaviour
{
	private void OnDestroy()
	{
		base.StopAllCoroutines();
	}

	public void OnApplicationQuit()
	{
		base.StopAllCoroutines();
	}

	public Coroutine ComStartCoroutine(IEnumerator ie)
	{
		return base.StartCoroutine(ie);
	}

	public void ComStopAllCoroutines()
	{
		base.StopAllCoroutines();
	}
}
