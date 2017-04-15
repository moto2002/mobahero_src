using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class OnEnableDelayerDestroy : MonoBehaviour
{
	public float delayerTime = 3f;

	private int enableCount;

	private void OnEnable()
	{
		this.enableCount++;
		if (this.enableCount == 2)
		{
			base.StartCoroutine(this.DestroyMe(this.delayerTime));
		}
	}

	[DebuggerHidden]
	private IEnumerator DestroyMe(float delayerTime)
	{
		OnEnableDelayerDestroy.<DestroyMe>c__Iterator1E5 <DestroyMe>c__Iterator1E = new OnEnableDelayerDestroy.<DestroyMe>c__Iterator1E5();
		<DestroyMe>c__Iterator1E.delayerTime = delayerTime;
		<DestroyMe>c__Iterator1E.<$>delayerTime = delayerTime;
		<DestroyMe>c__Iterator1E.<>f__this = this;
		return <DestroyMe>c__Iterator1E;
	}
}
