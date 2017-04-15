using System;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
	public ParticleSystem ps;

	public void OnSpawned()
	{
		if (this.ps != null)
		{
			this.ps.Play();
		}
	}

	private void Awake()
	{
		this.OnSpawned();
	}
}
