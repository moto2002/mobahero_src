using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAdapter : MonoBehaviour
{
	public int important = 1;

	public float countStep = 0.5f;

	public float rateStep = 0.8f;

	public ParticleSystem[] ps;

	public Renderer[] rd;

	public int[] countVault;

	public float[] rateVault;

	private bool[] checkCount;

	private bool[] checkRate;

	public int adaptRange = 1;

	public static List<ParticleAdapter> particles;

	private static SortImportant imp;

	private void Start()
	{
		if (ParticleAdapter.particles == null)
		{
			ParticleAdapter.particles = new List<ParticleAdapter>(15);
		}
		if (ParticleAdapter.imp == null)
		{
			ParticleAdapter.imp = new SortImportant();
		}
		ParticleAdapter.particles.Add(this);
		if (this.ps != null && this.ps.Length >= 1)
		{
			this.checkCount = new bool[this.ps.Length];
			this.checkRate = new bool[this.ps.Length];
			for (int i = 0; i < this.ps.Length; i++)
			{
				this.checkCount[i] = true;
				this.checkRate[i] = true;
			}
		}
		this.countStep = Mathf.Min(0.9f, this.countStep);
		this.rateStep = Mathf.Min(0.9f, this.rateStep);
		this.rd = base.GetComponentsInChildren<Renderer>();
	}

	private void OnDestroy()
	{
		if (ParticleAdapter.particles != null && ParticleAdapter.particles.Contains(this))
		{
			ParticleAdapter.particles.Remove(this);
		}
	}

	public bool ToDown()
	{
		int num = 0;
		for (int i = 0; i < this.ps.Length; i++)
		{
			int num2 = this.ps.Length - 1 - i;
			bool flag = false;
			if (this.checkCount[num2] || this.checkRate[num2])
			{
				flag = ParticleAdapter.AdaptDown(this.ps[num2], num2, this);
			}
			if (flag)
			{
				num++;
				if (num >= this.adaptRange)
				{
					break;
				}
			}
		}
		return num > 0;
	}

	private static bool AdaptDown(ParticleSystem p, int k, ParticleAdapter ad)
	{
		if (p == null || !p.enableEmission)
		{
			return false;
		}
		int num = 0;
		float num2 = 0f;
		if (k < ad.countVault.Length)
		{
			num = ad.countVault[k];
		}
		if (k < ad.rateVault.Length)
		{
			num2 = ad.rateVault[k];
		}
		int maxParticles = p.maxParticles;
		float emissionRate = p.emissionRate;
		if (maxParticles <= num && emissionRate <= num2)
		{
			return false;
		}
		int num3 = (int)(ad.countStep * (float)maxParticles);
		float num4 = ad.rateStep * emissionRate;
		if (num3 < num)
		{
			num3 = num;
			ad.checkCount[k] = false;
		}
		if (num4 < num2)
		{
			num4 = num2;
			ad.checkRate[k] = false;
		}
		if (num3 == 0)
		{
			p.enableEmission = false;
			ad.checkCount[k] = false;
		}
		else
		{
			p.maxParticles = num3;
		}
		p.emissionRate = num4;
		return true;
	}

	public static bool AdaptDown()
	{
		if (ParticleAdapter.particles == null || ParticleAdapter.particles.Count < 1)
		{
			return false;
		}
		ParticleAdapter.particles.Sort(ParticleAdapter.imp);
		bool flag = false;
		int num = 0;
		foreach (ParticleAdapter current in ParticleAdapter.particles)
		{
			if (current.important > num && flag)
			{
				break;
			}
			if (current.ToDown())
			{
				flag = true;
				num = current.important;
			}
		}
		return flag;
	}

	public void ShowRenders(bool b)
	{
		if (this.rd == null)
		{
			return;
		}
		for (int i = 0; i < this.rd.Length; i++)
		{
			if (this.rd[i] != null)
			{
				this.rd[i].enabled = b;
			}
		}
	}
}
