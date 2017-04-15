using System;
using UnityEngine;

public class AutoFadeOut : MonoBehaviour
{
	public ParticleSystem[] particles;

	public Renderer[] renders;

	public float time;

	[HideInInspector]
	public float lastTime;

	private void Awake()
	{
		this.particles = base.GetComponentsInChildren<ParticleSystem>();
		this.renders = base.GetComponentsInChildren<Renderer>();
		this.OnSpawned();
	}

	public void OnSpawned()
	{
		this.Fade(true);
		this.lastTime = Time.time;
	}

	public void OnDespawned()
	{
	}

	private void Update()
	{
		if (Time.time - this.lastTime > this.time)
		{
			this.lastTime = 3.40282347E+38f;
			this.Fade(false);
		}
	}

	private void Fade(bool b)
	{
		if (this.renders != null)
		{
			for (int i = 0; i < this.renders.Length; i++)
			{
			}
		}
		if (this.particles != null)
		{
			for (int j = 0; j < this.particles.Length; j++)
			{
				if (b)
				{
					this.particles[j].Play();
				}
				else
				{
					this.particles[j].Stop();
				}
			}
		}
	}
}
