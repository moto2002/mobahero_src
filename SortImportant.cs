using System;
using System.Collections.Generic;
/// <summary>
/// 粒子适配器重要程度排序比较类
/// </summary>
public class SortImportant : IComparer<ParticleAdapter>
{
	public int Compare(ParticleAdapter x, ParticleAdapter y)
	{
		if (x.important > y.important)
		{
			return 1;
		}
		if (x.important == y.important)
		{
			return 0;
		}
		if (x.important < y.important)
		{
			return -1;
		}
		return -1;
	}
}
