using System;
using UnityEngine;

public class SpellMono : MobaMono
{
	protected float getMaxParticleLife()
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		float num = 0f;
		bool flag = false;
		ParticleSystem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = array[i];
			if (particleSystem.loop)
			{
				flag = true;
				break;
			}
			if (particleSystem.duration >= num)
			{
				num = particleSystem.duration;
			}
		}
		float result;
		if (!flag)
		{
			result = num;
		}
		else
		{
			result = 10f;
		}
		return result;
	}
}
