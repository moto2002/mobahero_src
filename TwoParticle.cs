using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class TwoParticle : MonoBehaviour
{
	public GameObject LV2;

	private void OnEnable()
	{
		GlobalObject.Instance.StartCoroutine(this.SetLV2());
	}

	[DebuggerHidden]
	private IEnumerator SetLV2()
	{
		TwoParticle.<SetLV2>c__Iterator1E7 <SetLV2>c__Iterator1E = new TwoParticle.<SetLV2>c__Iterator1E7();
		<SetLV2>c__Iterator1E.<>f__this = this;
		return <SetLV2>c__Iterator1E;
	}

	private void OnDisable()
	{
		if (this.LV2 != null)
		{
			this.LV2.SetActive(false);
		}
	}
}
