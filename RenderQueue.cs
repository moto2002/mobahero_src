using System;
using UnityEngine;

public class RenderQueue : MonoBehaviour
{
	[SerializeField]
	private int renderQueue = 4000;

	[SerializeField]
	private Material[] mat;

	[SerializeField]
	private MeshRenderer meshRender;

	private ParticleSystem[] particles;

	private void Awake()
	{
		this.particles = base.GetComponentsInChildren<ParticleSystem>();
		if (this.particles != null)
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].renderer.material.renderQueue = this.renderQueue;
			}
		}
	}
}
