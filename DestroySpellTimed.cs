using System;
using UnityEngine;

public class DestroySpellTimed : SpellMono
{
	private void Awake()
	{
		UnityEngine.Object.Destroy(base.gameObject, base.getMaxParticleLife());
	}
}
