using System;
using System.Collections.Generic;

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
