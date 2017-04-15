using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ReturnController : MonoBehaviour
{
	public Transform Texture_ground;

	private void Start()
	{
	}

	public void WaitHide()
	{
		this.StopAll();
		base.StartCoroutine(this.HideBg());
	}

	public void StopAll()
	{
		base.StopAllCoroutines();
	}

	[DebuggerHidden]
	private IEnumerator HideBg()
	{
		return new ReturnController.<HideBg>c__IteratorA4();
	}
}
