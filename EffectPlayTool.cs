using System;
using UnityEngine;

public class EffectPlayTool : MonoBehaviour
{
	private ParticleSystem[] particle;

	public Animator[] animator;

	private void Awake()
	{
		this.particle = base.GetComponentsInChildren<ParticleSystem>();
		this.animator = base.GetComponentsInChildren<Animator>();
		this.Stop();
	}

	public void Play()
	{
		EffectQualityAdapter.SetReactiveAction(base.transform, new Action(this.Play));
		if (EffectQualityAdapter.IsEffectShutOff(base.transform))
		{
			return;
		}
		for (int i = 0; i < this.particle.Length; i++)
		{
			this.particle[i].Play();
		}
		for (int j = 0; j < this.animator.Length; j++)
		{
			this.animator[j].speed = 1f;
			this.animator[j].Play(0, 0, 0f);
		}
	}

	public void Stop()
	{
		EffectQualityAdapter.SetReactiveAction(base.transform, new Action(this.Stop));
		if (EffectQualityAdapter.IsEffectShutOff(base.transform))
		{
			return;
		}
		for (int i = 0; i < this.particle.Length; i++)
		{
			this.particle[i].Stop();
		}
		for (int j = 0; j < this.animator.Length; j++)
		{
			this.animator[j].speed = 0f;
			this.animator[j].Play(0, 0, 0f);
		}
	}

	public static void ShowEffect(bool isShow, GameObject obj)
	{
		ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
		Animator[] componentsInChildren2 = obj.GetComponentsInChildren<Animator>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (isShow)
			{
				componentsInChildren[i].Play();
			}
			else
			{
				componentsInChildren[i].Stop();
			}
		}
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].speed = (float)((!isShow) ? 0 : 1);
			componentsInChildren2[j].Play(0, 0, 0f);
		}
	}
}
