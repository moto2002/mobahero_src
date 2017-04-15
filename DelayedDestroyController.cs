using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class DelayedDestroyController : MonoBehaviour
{
	public float destroyDelay = 2.5f;

	private void Start()
	{
		base.StartCoroutine(this.DelayedDestroy());
	}

	[DebuggerHidden]
	protected IEnumerator DelayedDestroy()
	{
		DelayedDestroyController.<DelayedDestroy>c__Iterator1E4 <DelayedDestroy>c__Iterator1E = new DelayedDestroyController.<DelayedDestroy>c__Iterator1E4();
		<DelayedDestroy>c__Iterator1E.<>f__this = this;
		return <DelayedDestroy>c__Iterator1E;
	}
}
