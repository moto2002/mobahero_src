using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class QuickDestroy : MonoBehaviour
{
	private void Start()
	{
		base.StartCoroutine(this.DelayDestroy());
	}

	[DebuggerHidden]
	private IEnumerator DelayDestroy()
	{
		QuickDestroy.<DelayDestroy>c__Iterator128 <DelayDestroy>c__Iterator = new QuickDestroy.<DelayDestroy>c__Iterator128();
		<DelayDestroy>c__Iterator.<>f__this = this;
		return <DelayDestroy>c__Iterator;
	}

	private void Update()
	{
	}
}
