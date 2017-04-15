using System;
using UnityEngine;

public class ReplayEffect : MonoBehaviour
{
	[SerializeField]
	private float replayTime = 1f;

	private float lastTime;

	private ParticleSystem[] particles;

	private void Awake()
	{
		this.particles = base.GetComponentsInChildren<ParticleSystem>();
	}

	private void OnEnable()
	{
		this.lastTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - this.lastTime > this.replayTime)
		{
			this.lastTime = Time.time;
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Play();
			}
		}
	}
}
