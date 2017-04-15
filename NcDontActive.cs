using System;
using UnityEngine;

public class NcDontActive : NcEffectBehaviour
{
	private void Awake()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnEnable()
	{
	}
}
