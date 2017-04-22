using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 粒子适配器
/// </summary>
public class ParticleAdapter : MonoBehaviour
{
    /// <summary>
    /// 重要程度
    /// </summary>
	public int important = 1;
    /// <summary>
    /// 粒子数量限制控制
    /// </summary>
	public float countStep = 0.5f;
    /// <summary>
    /// 粒子发射速率控制限制
    /// </summary>
	public float rateStep = 0.8f;
    /// <summary>
    /// 相关粒子系统列表
    /// </summary>
	public ParticleSystem[] ps;
    /// <summary>
    /// 相关渲染器
    /// </summary>
	public Renderer[] rd;
    /// <summary>
    /// 适配器针对粒子系统列表的粒子数量限制
    /// </summary>
	public int[] countVault;
    /// <summary>
    ///  适配器针对粒子系统列表的粒子速率限制
    /// </summary>
	public float[] rateVault;
    /// <summary>
    /// 计数检查
    /// </summary>
	private bool[] checkCount;
    /// <summary>
    /// 速率检查
    /// </summary>
	private bool[] checkRate;
    /// <summary>
    /// 一次适配调整的最大粒子系统数量
    /// </summary>
	public int adaptRange = 1;
    /// <summary>
    /// 粒子适配器列表
    /// </summary>
	public static List<ParticleAdapter> particles;
    /// <summary>
    /// 重要程度排序
    /// </summary>
	private static SortImportant imp;
    /// <summary>
    /// 开始获取引用
    /// </summary>
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
    /// <summary>
    /// 移除所有粒子适配器列表单元项
    /// </summary>
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
			if (flag)//适配成功，计数加一
			{
				num++;
				if (num >= this.adaptRange)//达到适配次数上限范围，停止适配
				{
					break;
				}
			}
		}
		return num > 0;
	}
    /// <summary>
    /// 对指定粒子系统按指定适配器进行适配
    /// </summary>
    /// <param name="p"></param>
    /// <param name="k"></param>
    /// <param name="ad"></param>
    /// <returns></returns>
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
		if (maxParticles <= num && emissionRate <= num2)//没有越界，可以继续适配?????
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
    /// <summary>
    /// 优化戏能适配调整粒子系统属性
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// 渲染器启用或禁止
    /// </summary>
    /// <param name="b"></param>
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
