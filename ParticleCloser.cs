using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ParticleCloser : MonoBehaviour
{
	public float closeTime = 5f;

	private ParticleSystem[] ps;

	private void Start()
	{
		this.ps = base.GetComponentsInChildren<ParticleSystem>();
		base.StartCoroutine(this.Closer());
	}

	private void Update()
	{
	}

	[DebuggerHidden]
	private IEnumerator Closer()
	{
		ParticleCloser.<Closer>c__Iterator1E6 <Closer>c__Iterator1E = new ParticleCloser.<Closer>c__Iterator1E6();
		<Closer>c__Iterator1E.<>f__this = this;
		return <Closer>c__Iterator1E;
	}
}
